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
    public partial class MatchResultsControl : UserControl
    {
        #region Private Variables
        /// <summary>
        /// The function to call when the "Continue" button is clicked.
        /// </summary>
        private Action<object, EventArgs> onCloseFunc = null;

        /// <summary>
        /// The match being presented by this results screen.
        /// </summary>
        private Match match;
        #endregion

        #region Public methods
        /// <summary>
        /// Returns the match being handled by this results screen.
        /// </summary>
        /// <returns></returns>
        public Match getMatch()
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
            winningTeamLabel.Text = m.getTeamName(m.winner);
            team1Info.Text = m.getFormattedLineup(1);
            team2Info.Text = m.getFormattedLineup(2);
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
        #endregion
    }
}
