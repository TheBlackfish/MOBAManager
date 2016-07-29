using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOBAManager.Management.Teams;
using MOBAManager.Management.Calendar;
using MOBAManager.Utility;

namespace MOBAManager.UI.Calendar
{
    sealed public partial class CalendarScheduleControl : UserControl
    {
        #region Private Variables
        /// <summary>
        /// The calendar manager of the schedule control
        /// </summary>
        private readonly CalendarManager cm;

        /// <summary>
        /// The team manager of the team control
        /// </summary>
        private readonly TeamManager tm;

        /// <summary>
        /// The offset that the schedule control represents
        /// </summary>
        private readonly int dayOffset;

        /// <summary>
        /// The list of all possible events possible on the offset that this schedule control represents.
        /// </summary>
        private readonly List<CalendarEvent> possibleEvents;

        /// <summary>
        /// The function called to pass any chosen event onto the calender manager.
        /// </summary>
        private readonly Action<CalendarEvent> submissionFunction;
        #endregion

        #region Private methods
        /// <summary>
        /// Creates a list of all possible events that the player can schedule on the date represented by this schedule control.
        /// </summary>
        private void CreateAllPossibleEvents()
        {
            //Get all possible PUGs from cm and create events for them.
            foreach (Team t in tm.GetAllTeams())
            {
                if (t.ID != 0)
                {
                    if (!cm.TeamHasEventsOnDate(t.ID, dayOffset))
                    {
                        if (RNG.CoinFlip())
                        {
                            possibleEvents.Add(new CalendarEvent(EventType.PUG, dayOffset, 0, t.ID));
                        }
                        else
                        {
                            possibleEvents.Add(new CalendarEvent(EventType.PUG, dayOffset, t.ID, 0));
                        }
                    }
                }
            }

            possibleEvents.Add(new CalendarEvent(EventType.Bootcamp, dayOffset, 0, -1));
        }

        /// <summary>
        /// Creates clickable buttons for each possible event found by this schedule control.
        /// </summary>
        private void CreateAllButtons()
        {
            int curButtonOffset = 0;

            for (int i = 0; i < possibleEvents.Count; i++)
            {
                CalendarEvent cur = possibleEvents[i];

                string desc = "?";
                if (cur.type == EventType.PUG)
                {
                    if (cur.team1ID == 0)
                    {
                        desc = "Schedule a pick-up game with " + tm.GetTeamName(cur.team2ID);
                    }
                    else
                    {
                        desc = "Schedule a pick-up game with " + tm.GetTeamName(cur.team1ID);
                    }
                }
                else if (cur.type == EventType.Bootcamp)
                {
                    desc = "Schedule a bootcamp.";
                }

                Button btn = new Button();
                btn.Text = desc;
                btn.Location = new Point(0, curButtonOffset);
                btn.MaximumSize = new Size(buttonContainer.ClientSize.Width - 6, 0);
                btn.MinimumSize = new Size(buttonContainer.ClientSize.Width - 6, 40);
                btn.Margin = new Padding(3);

                //Add button functionality
                int eventIndex = i;
                Action<object, EventArgs> action = (object sender, EventArgs e) =>
                {
                    submissionFunction(possibleEvents[eventIndex]);
                };
                btn.Click += new EventHandler(action);

                buttonContainer.Controls.Add(btn);
                curButtonOffset += btn.Size.Height;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new schedule control.
        /// </summary>
        /// <param name="cm">The calendar manager this control coorresponds with.</param>
        /// <param name="tm">The team manager this control cooresponds with.</param>
        /// <param name="offset">The offset that cooresponds with the day this schedule control represents.</param>
        /// <param name="submit">The function that should be called when an event is chosen.</param>
        public CalendarScheduleControl(CalendarManager cm, TeamManager tm, int offset, Action<CalendarEvent> submit)
        {
            InitializeComponent();
            this.cm = cm;
            this.tm = tm;
            dayOffset = offset;
            submissionFunction = submit;
            possibleEvents = new List<CalendarEvent>();
        }
        #endregion

        /// <summary>
        /// Called when the parent of the control changes. Centers the control and creates all possible events while adding UI for them to itself.
        /// </summary>
        private void CalendarScheduleControl_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Location = new Point((Parent.ClientSize.Width - Size.Width) / 2, (Parent.ClientSize.Height - Size.Height) / 2);
                CreateAllPossibleEvents();
                CreateAllButtons();
            }
        }

        /// <summary>
        /// Called when the return button is clicked. Closes the control.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Parent.Controls.Remove(this);
        }
    }
}
