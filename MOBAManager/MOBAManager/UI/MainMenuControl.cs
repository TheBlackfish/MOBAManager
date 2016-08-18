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
            continueButton.Enabled = File.Exists("save.xml");
        }

        /// <summary>
        /// Called when the 'New Game' button is clicked. Creates a new game and displays it for the player.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (Parent is MainForm)
            {
                ((MainForm)Parent).RunGame();
            }
        }

        /// <summary>
        /// Called when the 'Continue' button is clicked. Loads the game from the default save location and displays it for the player.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (Parent is MainForm)
            {
                ((MainForm)Parent).RunGame("save.xml");
            }
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
        /// Called when the main menu's parent changes. Centers the menu panel in the middle of the parent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainMenuControl_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Size = new Size(Parent.ClientSize.Width, Parent.ClientSize.Height);
                centerPanel.Location = new Point((Parent.ClientSize.Width - centerPanel.Size.Width) / 2, (Parent.ClientSize.Height - centerPanel.Size.Height) / 2);
            }
        }
    }
}
