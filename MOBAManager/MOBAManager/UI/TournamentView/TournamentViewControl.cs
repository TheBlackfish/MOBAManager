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
    public partial class TournamentViewControl : UserControl
    {
        #region Private variables
        /// <summary>
        /// The tournament manager for this view control.
        /// </summary>
        private TournamentManager tm;

        /// <summary>
        /// The list of tournaments in this view control.
        /// </summary>
        private List<Tournament> tournamentStorage;

        /// <summary>
        /// The function to close this control.
        /// </summary>
        private Action onCloseFunc = null;
        #endregion

        #region Public methods
        /// <summary>
        /// Closes the detail control.
        /// </summary>
        public void closeDetail()
        {
            foreach (Control c in Controls)
            {
                if (c is TournamentDetailControl)
                {
                    Controls.Remove(c);
                }
            }
        }
        #endregion

        #region Private variables
        /// <summary>
        /// Displays the buttons for each tournament in the storage.
        /// </summary>
        private void prepareContentPanel()
        {
            tournamentStorage = tm.GetIncompleteTournaments();

            buttonGrid.SuspendLayout();
            buttonGrid.RowCount = tournamentStorage.Count();

            int currentRowNum = 0;
            foreach (Tournament t in tournamentStorage)
            {
                Button viewTournament = new Button();
                viewTournament.Text = t.name;
                viewTournament.Size = new Size(buttonGrid.ClientSize.Width, 40);
                viewTournament.Margin = new Padding(0);

                int curNum = currentRowNum;

                Action<object, EventArgs> action = (object sender, EventArgs e) =>
                {
                    ShowTournamentDetail(curNum);
                };
                viewTournament.Click += new EventHandler(action);

                buttonGrid.Controls.Add(viewTournament, 0, currentRowNum);
                currentRowNum++;
            }
            buttonGrid.Size = new Size(buttonGrid.Size.Width, tournamentStorage.Count * 40);
            buttonGrid.ResumeLayout();
        }

        /// <summary>
        /// Displays the detail for the tournament.
        /// </summary>
        /// <param name="num">The index of the tournament to display in the tournament storage.</param>
        private void ShowTournamentDetail(int num)
        {
            TournamentDetailControl tdc = new TournamentDetailControl(tournamentStorage[num], closeDetail);
            Controls.Add(tdc);
            tdc.BringToFront();
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new tournament view control.
        /// </summary>
        /// <param name="tm">The tournament manager to display.</param>
        /// <param name="onCloseFunc">The function that closes the control.</param>
        public TournamentViewControl(TournamentManager tm, Action onCloseFunc)
        {
            InitializeComponent();
            this.tm = tm;
            this.onCloseFunc = onCloseFunc;
            tournamentStorage = new List<Tournament>();
            prepareContentPanel();
        }
        #endregion

        #region Event responses
        /// <summary>
        /// Called when the parent changes. Resizes the control to fit its parent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TournamentViewControl_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Size = Parent.ClientSize;
            }
        }

        /// <summary>
        /// Called when the control resizes. Resizes all the buttons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TournamentViewControl_Resize(object sender, EventArgs e)
        {
            buttonGrid.SuspendLayout();
            foreach (Control c in buttonGrid.Controls)
            {
                c.Size = new Size(buttonGrid.ClientSize.Width, 40);
            }
            buttonGrid.ResumeLayout();
        }

        /// <summary>
        /// Called when the button is clicked. Calls the closing function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            onCloseFunc?.Invoke();
        }
        #endregion
    }
}
