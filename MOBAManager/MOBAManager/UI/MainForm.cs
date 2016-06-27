using MOBAManager.Management;
using MOBAManager.MatchResolution;
using MOBAManager.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MOBAManager
{
    public partial class MainForm : Form
    {
        private GameManager gm;
        private MatchResolutionControl mrc;
        

        public MainForm()
        {
            InitializeComponent();
            gm = new GameManager();
            showNextPlayerMatch();
        }

        private void showNextPlayerMatch()
        {
            Match m = gm.getNextPlayerMatch();
            mrc = new MatchResolutionControl(m);
            Controls.Add(mrc);
        }
    }
}
