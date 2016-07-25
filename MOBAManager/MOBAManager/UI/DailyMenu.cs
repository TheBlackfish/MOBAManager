using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOBAManager.Management;
using MOBAManager.MatchResolution;
using MOBAManager.UI.Meta;
using MOBAManager.Management.Calendar;
using System.Threading;
using MOBAManager.UI.Calendar;

namespace MOBAManager.UI
{
    public partial class DailyMenu : UserControl
    {
        #region Private variables
        /// <summary>
        /// The game manager this menu corresponds to.
        /// </summary>
        private GameManager gameManager;
        #endregion

        #region Private methods
        /// <summary>
        /// Enables or disables all buttons with interactivity.
        /// </summary>
        /// <param name="enabled"></param>
        private void setButtonStates(bool enabled)
        {
            metaButton.Enabled = enabled;
            resolutionButton.Enabled = enabled;
        }

        /// <summary>
        /// Finishes the daily resolution by providing data to the appropriate managers.
        /// </summary>
        private void resolveDailyResolution()
        {
            foreach (Control c in Controls)
            {
                if (c is EventResolutionControl)
                {
                    gameManager.statsManager.processManyBundles(((EventResolutionControl)c).getStatistics(), gameManager.heroManager, gameManager.playerManager, gameManager.teamManager);
                    c.Hide();
                    Controls.Remove(c);
                    setButtonStates(true);
                }
            }
            gameManager.calendarManager.incrementCalender();
            gameManager.calendarManager.scheduleRandomEventsForEachAITeam();
            label1.Text = gameManager.calendarManager.getFormattedDate();
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new menu.
        /// </summary>
        /// <param name="gm">The game manager this menu corresponds to.</param>
        public DailyMenu(GameManager gm)
        {
            InitializeComponent();
            gameManager = gm;
        }
        #endregion

        #region Event responses
        /// <summary>
        /// Called when the control's parent changes. Resizes the control to fit its parent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DailyMenu_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Size = Parent.ClientSize;
            }
            label1.Text = gameManager.calendarManager.getFormattedDate();
        }

        /// <summary>
        /// Called when the META button is clicked. Brings up the statistics control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metaButton_Click(object sender, EventArgs e)
        {
            Action closeStats = () =>
            {
                foreach (Control c in Controls)
                {
                    if (c is StatisticsControl)
                    {
                        c.Hide();
                        Controls.Remove(c);
                        setButtonStates(true);
                    }
                }
            };
            StatisticsControl sc = new StatisticsControl(gameManager.statsManager, closeStats);

            //Display statistics
            setButtonStates(false);
            Controls.Add(sc);
            sc.BringToFront();
        }

        /// <summary>
        /// Called when the Resolution button is clicked. Brings up the event resolution control and provides it with all the events for the day.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resolutionButton_Click(object sender, EventArgs e)
        {
            List<Match> pickupgames = new List<Match>();

            foreach (CalendarEvent ce in gameManager.calendarManager.getTodaysEvents())
            {
                switch (ce.type)
                {
                    case CalendarEvent.EventType.PUG:
                        pickupgames.Add(gameManager.translateEventToMatch(ce));
                        break;
                }
            }

            EventResolutionControl erc = new EventResolutionControl("Events on " + gameManager.calendarManager.getFormattedDate(), pickupgames, resolveDailyResolution);

            //Display event manager
            setButtonStates(false);
            Controls.Add(erc);
            erc.BringToFront();
        }

        /// <summary>
        /// Called when the Calendar button is clicked. Brings up the calendar screen and provides it with the appropriate managers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calendarButton_Click(object sender, EventArgs e)
        {
            Action closeStats = () =>
            {
                foreach (Control c in Controls)
                {
                    if (c is CalendarViewControl)
                    {
                        c.Hide();
                        Controls.Remove(c);
                        setButtonStates(true);
                    }
                }
            };
            CalendarViewControl cvc = new CalendarViewControl(gameManager.calendarManager, gameManager.teamManager, closeStats);

            setButtonStates(false);
            Controls.Add(cvc);
            cvc.BringToFront();
        }
        #endregion


    }
}
