using MOBAManager.Management;
using MOBAManager.MatchResolution;
using MOBAManager.UI;
using MOBAManager.Utility;
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

        public MainForm()
        {
            InitializeComponent();

            RNG.initRNG();
            
            gm = new GameManager();

            DailyMenu dm = new DailyMenu(gm);

            Controls.Add(dm);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //Add fullscreen eventually.
        }
    }
}
