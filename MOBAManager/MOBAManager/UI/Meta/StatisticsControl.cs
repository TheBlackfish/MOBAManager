using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOBAManager.Management.Statistics;
using System.Threading;

namespace MOBAManager.UI.Meta
{
    sealed public partial class StatisticsControl : UserControl
    {
        #region Private variables
        /// <summary>
        /// The statistics manager this control displays data for.
        /// </summary>
        private readonly StatisticsManager sm;

        private readonly Action onCloseFunc = null;
        #endregion

        #region Private methods
        /// <summary>
        /// Displays the appropriate statistics in the hero data grid view.
        /// </summary>
        private void DisplayHeroStats()
        {
            Action addStats = () =>
            {
                BindingSource src = sm.GetHeroStats();
                heroDataGridView.AutoGenerateColumns = false;
                heroDataGridView.AutoSize = true;
                heroDataGridView.DataSource = src;

                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.DataPropertyName = "Item1";
                column.Name = "Hero";
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                heroDataGridView.Columns.Add(column);

                column = new DataGridViewTextBoxColumn();
                column.DataPropertyName = "Item2";
                column.Name = "Win Rate";
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                heroDataGridView.Columns.Add(column);

                column = new DataGridViewTextBoxColumn();
                column.DataPropertyName = "Item3";
                column.Name = "Games Banned";
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                heroDataGridView.Columns.Add(column);

                column = new DataGridViewTextBoxColumn();
                column.DataPropertyName = "Item4";
                column.Name = "Games Picked";
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                heroDataGridView.Columns.Add(column);

                heroDataGridView.Invalidate();
            };
            BeginInvoke(addStats);

        }

        /// <summary>
        /// Displays the appropriate statistics in the player data grid view.
        /// </summary>
        private void DisplayPlayerStats()
        {
            Action addStats = () =>
            {
                BindingSource src = sm.GetPlayerStats();
                playerDataGridView.AutoGenerateColumns = false;
                playerDataGridView.AutoSize = true;
                playerDataGridView.DataSource = src;

                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.DataPropertyName = "Item1";
                column.Name = "Player";
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                playerDataGridView.Columns.Add(column);

                column = new DataGridViewTextBoxColumn();
                column.DataPropertyName = "Item2";
                column.Name = "Win Rate";
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                playerDataGridView.Columns.Add(column);

                column = new DataGridViewTextBoxColumn();
                column.DataPropertyName = "Item3";
                column.Name = "Games Played";
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                playerDataGridView.Columns.Add(column);

                playerDataGridView.Invalidate();
            };
            BeginInvoke(addStats);
        }

        /// <summary>
        /// Displays the appropriate statistics in the team data grid view.
        /// </summary>
        private void DisplayTeamStats()
        {
            Action addStats = () =>
            {
                BindingSource src = sm.GetTeamStats();
                teamDataGridView.AutoGenerateColumns = false;
                teamDataGridView.AutoSize = true;
                teamDataGridView.DataSource = src;

                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.DataPropertyName = "Item1";
                column.Name = "Team";
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                teamDataGridView.Columns.Add(column);

                column = new DataGridViewTextBoxColumn();
                column.DataPropertyName = "Item2";
                column.Name = "Win Rate";
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                teamDataGridView.Columns.Add(column);

                column = new DataGridViewTextBoxColumn();
                column.DataPropertyName = "Item3";
                column.Name = "Games Played";
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                teamDataGridView.Columns.Add(column);

                teamDataGridView.Invalidate();
            };
            BeginInvoke(addStats);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new statistics control.
        /// </summary>
        /// <param name="stats">The manager for the statistics to display.</param>
        public StatisticsControl(StatisticsManager stats, Action closeFunc)
        {
            InitializeComponent();
            onCloseFunc = closeFunc;
            sm = stats;
        }
        #endregion

        #region Event responses
        /// <summary>
        /// Called when a tab is selected. Generates display data for the tab if none exists currently.
        /// </summary>
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            switch (e.TabPageIndex)
            {
                case 0:
                    if (heroDataGridView.DataSource == null)
                    {
                        Task.Run(() => DisplayHeroStats());
                    }
                    break;
                case 1:
                    if (playerDataGridView.DataSource == null)
                    {
                        Task.Run(() => DisplayPlayerStats());
                    }
                    break;
                case 2:
                    if (teamDataGridView.DataSource == null)
                    {
                        Task.Run(() => DisplayTeamStats());
                    }
                    break;
            }
        }

        /// <summary>
        /// Called when the control's parent changes. Resizes the control to fit its parent.
        /// </summary>
        private void StatisticsControl_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Size = Parent.Size;
            }
        }

        /// <summary>
        /// Called the the control is fully loaded. (OH YEAH) Displays hero display data because that's the default tab in our control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatisticsControl_Load(object sender, EventArgs e)
        {
            Task.Run(() => DisplayHeroStats());
        }

        /// <summary>
        /// Called when the Return button is clicked. Hides and removes the control from the daily menu control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitButton_Click(object sender, EventArgs e)
        {
            onCloseFunc?.Invoke();
        }
        #endregion
    }
}
