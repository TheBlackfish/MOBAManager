using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOBAManager.MatchResolution;

namespace MOBAManager.UI
{
    sealed public partial class MatchResultsControl : UserControl
    {
        #region Private Variables
        /// <summary>
        /// The function to call when the "Continue" button is clicked.
        /// </summary>
        private readonly Action<object, EventArgs> onCloseFunc = null;

        /// <summary>
        /// The match being presented by this results screen.
        /// </summary>
        private readonly Match match;
        #endregion

        #region Public methods
        /// <summary>
        /// Returns the match being handled by this results screen.
        /// </summary>
        /// <returns></returns>
        public Match GetMatch()
        {
            return match;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the results pop-up.
        /// </summary>
        /// <param name="m"></param>
        public MatchResultsControl(Match m)
        {
            InitializeComponent();

            match = m;
            winningTeamLabel.Text = m.GetTeamName(m.Winner);
            team1Info.Text = m.GetLineupDisplayInformation(1);
            team2Info.Text = m.GetLineupDisplayInformation(2);
        }

        /// <summary>
        /// Creates the results pop-up with a closing function.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="closeFunc"></param>
        public MatchResultsControl(Match m, Action<object, EventArgs> closeFunc)
            : this(m)
        {
            onCloseFunc = closeFunc;
        }
        #endregion

        #region Event Responses
        /// <summary>
        /// Invokes the function provided to the results screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void continueButton_Click(object sender, EventArgs e)
        {
            onCloseFunc?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Called when this control's parent is changed. Expands the background to fit the parent and then centers all information in the center of the control.
        /// </summary>
        private void MatchResultsControl_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Size = Parent.ClientSize;
                centerPanel.Location = new Point(
                        (Size.Width - centerPanel.Size.Width) / 2,
                        (Size.Height - centerPanel.Size.Height) / 2
                    );
                centerPanel.Invalidate();
            }
        }
        #endregion
    }
}
