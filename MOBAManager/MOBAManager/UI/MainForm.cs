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
using System.Xml.Linq;

namespace MOBAManager
{
    sealed public partial class MainForm : Form
    {
        public bool currentlyRunningGame = false;

        private DailyMenu gameView;
        private MainMenuControl menuView;

        public MainForm()
        {
            InitializeComponent();

            RNG.InitRNG();

            menuView = new MainMenuControl();

            Controls.Add(menuView);
        }

        public void RunGame()
        {
            currentlyRunningGame = true;
            GameManager gm = new GameManager();
            gameView = new DailyMenu(gm);
            if (!Controls.Contains(gameView))
            {
                Controls.Add(gameView);
            }
            gameView.BringToFront();
        }

        public void RunGame(string fileLocation)
        {
            if (currentlyRunningGame)
            {
                gameView.BringToFront();
            }
            else
            {
                currentlyRunningGame = true;
                GameManager gm = new GameManager(XDocument.Load(fileLocation));
                gameView = new DailyMenu(gm);
                if (!Controls.Contains(gameView))
                {
                    Controls.Add(gameView);
                }
                gameView.BringToFront();
            }
            
        }
    }
}
