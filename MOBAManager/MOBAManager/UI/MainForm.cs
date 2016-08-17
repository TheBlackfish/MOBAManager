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
        public MainForm()
        {
            InitializeComponent();

            RNG.InitRNG();

            MainMenuControl mmc = new MainMenuControl();

            Controls.Add(mmc);
        }
    }
}
