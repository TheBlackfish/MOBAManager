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
        #endregion

        #region Private methods
        /// <summary>
        /// Updates the textboxes with the appropriate data from the match.
        /// </summary>
        private void updateTimer_Tick(object sender, EventArgs e)
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
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new MRC user control.
        /// </summary>
        /// <param name="m">The match that this MRC is handling.</param>
        public MatchResolutionControl(Match m)
        {
            match = m;
            InitializeComponent();
            team1Info.Text = m.getTeamInformation(1);
            team2Info.Text = m.getTeamInformation(2);
        }
        #endregion
    }
}