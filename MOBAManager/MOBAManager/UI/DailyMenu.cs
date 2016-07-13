using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOBAManager.Management;
using MOBAManager.MatchResolution;

namespace MOBAManager.UI
{
    public partial class DailyMenu : UserControl
    {
        private GameManager gameManager;

        private void setButtonStates(bool enabled)
        {
            resolutionButton.Enabled = enabled;
        }

        public DailyMenu(GameManager gm)
        {
            InitializeComponent();
            gameManager = gm;
        }

        private void resolutionButton_Click(object sender, EventArgs e)
        {
            //Add events to event manager
            List<Match> pickupgames = new List<Match>();

            pickupgames.Add(gameManager.getNextPlayerMatch());
            for (int i = 0; i < 20; i++)
            {
                pickupgames.Add(gameManager.getNextMatch());
            }

            EventResolutionControl erc = new EventResolutionControl(pickupgames);

            //Display event manager
            setButtonStates(false);
            Controls.Add(erc);
            erc.BringToFront();
        }
    }
}
