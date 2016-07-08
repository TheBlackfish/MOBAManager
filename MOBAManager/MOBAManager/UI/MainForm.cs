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
        private EventResolutionControl erc;        

        public MainForm()
        {
            InitializeComponent();

            Random rng = new Random();
            gm = new GameManager();

            List<Match> testPugs = new List<Match>();
            testPugs.Add(new Match(true, gm.teamManager.getAllTeams()[0], gm.teamManager.getAllTeams()[1], 1, gm.heroManager.getHeroDictionary()));

            for (int i = 0; i < 5; i++)
            {
                var team1Index = rng.Next(gm.teamManager.getAllTeams().Count);
                var team2Index = team1Index;

                while (team2Index == team1Index)
                {
                    team2Index = rng.Next(gm.teamManager.getAllTeams().Count);
                }

                if (team1Index == 0)
                {
                    testPugs.Add(new Match(true, gm.teamManager.getAllTeams()[team1Index], gm.teamManager.getAllTeams()[team2Index], 1, gm.heroManager.getHeroDictionary()));
                }
                else if (team2Index == 0)
                {
                    testPugs.Add(new Match(true, gm.teamManager.getAllTeams()[team1Index], gm.teamManager.getAllTeams()[team2Index], 2, gm.heroManager.getHeroDictionary()));
                }
                else
                {
                    testPugs.Add(new Match(gm.teamManager.getAllTeams()[team1Index], gm.teamManager.getAllTeams()[team2Index], gm.heroManager.getHeroDictionary()));
                }
            }

            erc = new EventResolutionControl(testPugs);
            Controls.Add(erc);
        }
    }
}
