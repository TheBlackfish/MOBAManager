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
    sealed public partial class MainForm : Form
    {
        private readonly GameManager gm;      

        public MainForm()
        {
            InitializeComponent();

            RNG.InitRNG();
            
            gm = new GameManager();

            DailyMenu dm = new DailyMenu(gm);

            Controls.Add(dm);
        }
    }
}
