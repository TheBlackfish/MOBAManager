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
    public partial class MatchResultsControl : UserControl
    {
        public MatchResultsControl(Match m)
        {
            InitializeComponent();

            winningTeamLabel.Text = m.getTeamName(m.winner);
            team1Info.Text = m.getFormattedLineup(1);
            team2Info.Text = m.getFormattedLineup(2);
        }
    }
}
