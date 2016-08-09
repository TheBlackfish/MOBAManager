using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOBAManager.Management.Tournaments;

namespace MOBAManager.UI.TournamentView
{
    public partial class TournamentDetailControl : UserControl
    {
        #region Private variables
        /// <summary>
        /// The function to close when the Return button is clicked.
        /// </summary>
        private Action onCloseFunc = null;
        #endregion

        #region Private methods
        /// <summary>
        /// Displays the tournament on the control.
        /// </summary>
        /// <param name="t">The tournament to display.</param>
        private void ShowTournament(Tournament t)
        {
            TableLayoutPanel tlp = t.GetDisplayPanel();
            contentPanel.Controls.Add(tlp);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new tournament detail control.
        /// </summary>
        /// <param name="toDisplay">The tournament to display.</param>
        /// <param name="onCloseFunc">The function to close the control.</param>
        public TournamentDetailControl(Tournament toDisplay, Action onCloseFunc)
        {
            InitializeComponent();
            this.onCloseFunc = onCloseFunc;
            ShowTournament(toDisplay);
        }
        #endregion

        #region Event responses
        /// <summary>
        /// Called when the button is clicked. Calls the closing function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            onCloseFunc?.Invoke();
        }

        /// <summary>
        /// Called when the parent is changed. Resizes the control to fit its parent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TournamentDetailControl_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Size = Parent.ClientSize;
            }
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TournamentDetailControl_Resize(object sender, EventArgs e)
        {
            //Blank for now.
        }
        #endregion
    }
}
