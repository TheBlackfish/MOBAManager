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

namespace MOBAManager.UI
{
    public partial class MatchResolutionControl : UserControl
    {
        private Match match;

        public MatchResolutionControl(Match m)
        {
            match = m;
            InitializeComponent();
        }

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
    }
}
