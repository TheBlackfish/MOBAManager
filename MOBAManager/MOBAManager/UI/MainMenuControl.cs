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
using System.IO;
using System.Xml.Linq;

namespace MOBAManager.UI
{
    public partial class MainMenuControl : UserControl
    {
        /// <summary>
        /// Constructs the MainMenuControl while checking for any existing save files.
        /// </summary>
        public MainMenuControl()
        {
            InitializeComponent();
            button2.Enabled = File.Exists("save.xml");
        }

        /// <summary>
        /// Called when the 'New Game' button is clicked. Creates a new game and displays it for the player.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            GameManager gm = new GameManager();
            DailyMenu dm = new DailyMenu(gm);
            Parent.Controls.Add(dm);
            dm.BringToFront();
        }

        /// <summary>
        /// Called when the 'Continue' button is clicked. Loads the game from the default save location and displays it for the player.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            GameManager gm = new GameManager(XDocument.Load("save.xml"));
            DailyMenu dm = new DailyMenu(gm);
            Parent.Controls.Add(dm);
            dm.BringToFront();
        }

        /// <summary>
        /// Called when the 'Quit' button is clicked. Exits the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            //Do any before-quitting things here.
            Application.Exit();
        }

        /// <summary>
        /// Called when the main menu's parent changes. Centers the menu in the middle of the parent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainMenuControl_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Location = new Point((Parent.ClientSize.Width - Size.Width) / 2, (Parent.ClientSize.Height - Size.Height) / 2);
            }
        }
    }
}
