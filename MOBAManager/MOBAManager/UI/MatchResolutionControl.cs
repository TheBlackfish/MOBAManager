using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOBAManager.MatchResolution;
using System.Runtime.InteropServices;

namespace MOBAManager.UI
{
    public partial class MatchResolutionControl : UserControl
    {
        #region Private variables
        /// <summary>
        /// The match this MRC controls.
        /// </summary>
        private Match match;

        /// <summary>
        /// The function to call when the results screen is closed.
        /// </summary>
        private Action<object, EventArgs> onCloseResultsFunc;

        /// <summary>
        /// The list of action combinations for selection recommendations.
        /// </summary>
        private List<Tuple<int, int>> interactionsList;

        /// <summary>
        /// The internal timer for updating the screen.
        /// </summary>
        private System.Timers.Timer updateTimer;
        #endregion

        #region Private methods
        /// <summary>
        /// Updates the textboxes with the appropriate data from the match.
        /// </summary>
        private void updateTimer_Tick(object sender, EventArgs e)
        {
            Action update = () => updateControl();
            BeginInvoke(update);
        }

        /// <summary>
        /// Updates all text and timers.
        /// </summary>
        private void updateControl()
        {
            Tuple<string, string, string, string> times = match.getFormattedTimers();
            team1TimerA.Text = times.Item1;
            team1TimerB.Text = times.Item2;
            team2TimerA.Text = times.Item3;
            team2TimerB.Text = times.Item4;

            if (match.hasChanged)
            {
                team1Bans.Text = match.getFormattedTeam1Bans();
                team1Picks.Text = match.getFormattedTeam1Picks();
                team2Bans.Text = match.getFormattedTeam2Bans();
                team2Picks.Text = match.getFormattedTeam2Picks();
            }

            if (match.hasFinished)
            {
                updateTimer.Enabled = false;

                EventResolutionControl form = this.Parent as EventResolutionControl;
                form.Controls.Remove(this);

                MatchResultsControl results = new MatchResultsControl(match, onCloseResultsFunc);
                form.Controls.Add(results);
                results.BringToFront();
            }
        }
        #endregion

        /// <summary>
        /// Returns the match this control is handling.
        /// </summary>
        /// <returns></returns>
        public Match getMatch()
        {
            return match;
        }

        #region Constructors
        /// <summary>
        /// Creates a new MRC user control.
        /// </summary>
        /// <param name="m">The match that this MRC is handling.</param>
        public MatchResolutionControl(Match m)
        {
            match = m;
            InitializeComponent();

            updateTimer = new System.Timers.Timer(250);
            updateTimer.Elapsed += updateTimer_Tick;
            updateTimer.Enabled = true;

            team1Info.Text = m.getTeamInformation(1);
            team2Info.Text = m.getTeamInformation(2);

            interactionsList = new List<Tuple<int, int>>();

            foreach (Tuple<string, int, int> t in m.getPlayerInteractions())
            {
                userSelection.Items.Add(t.Item1);
                interactionsList.Add(new Tuple<int, int>(t.Item2, t.Item3));
            }

            m.startMatch();
        }

        /// <summary>
        /// Creates a new MRC user control with a function to call upon finishing.
        /// </summary>
        /// <param name="m">The match that this MRC is handling.</param>
        /// <param name="onEndFunc">The function to call when the match is over.</param>
        public MatchResolutionControl(Match m, Action<object, EventArgs> onEndFunc)
            : this(m)
        {
            onCloseResultsFunc = onEndFunc;
        }
        #endregion

        /// <summary>
        /// Called when the combo box's selection changes.
        /// </summary>
        /// <param name="sender">The combo box.</param>
        /// <param name="e">The event parameters.</param>
        private void userSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            Tuple<int, int> selection = interactionsList[userSelection.SelectedIndex];
            match.submitPlayerRecommendation(selection.Item1, selection.Item2);
        }

        private void MatchResolutionControl_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Size = Parent.Size;
                centerPanel.Location = new Point(
                        (Size.Width - centerPanel.Size.Width) / 2,
                        (Size.Height - centerPanel.Size.Height) / 2
                    );
                centerPanel.Invalidate();
            }
        }
    }
}