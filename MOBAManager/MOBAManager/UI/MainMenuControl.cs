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
        public MainMenuControl()
        {
            InitializeComponent();
            button2.Enabled = File.Exists("save.xml");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GameManager gm = new GameManager();
            DailyMenu dm = new DailyMenu(gm);
            Parent.Controls.Add(dm);
            dm.BringToFront();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GameManager gm = new GameManager(XDocument.Load("save.xml"));
            DailyMenu dm = new DailyMenu(gm);
            Parent.Controls.Add(dm);
            dm.BringToFront();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Do any before-quitting things here.
            Application.Exit();
        }

        private void MainMenuControl_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Location = new Point((Parent.ClientSize.Width - Size.Width) / 2, (Parent.ClientSize.Height - Size.Height) / 2);
            }
        }
    }
}
