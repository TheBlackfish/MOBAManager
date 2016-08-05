using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOBAManager.Management.Players;
using MOBAManager.Management.Teams;
using MOBAManager.Resolution.BootcampResolution;

namespace MOBAManager.UI.DailyResolution
{
    public partial class BootcampResolutionControl : UserControl
    {
        #region Private variables
        /// <summary>
        /// Control variable for if the bootcamp has actually resolved yet or not.
        /// </summary>
        private bool hasTrained = false;

        /// <summary>
        /// The bootcamp that this control represents.
        /// </summary>
        private BootcampSession bm;

        /// <summary>
        /// The list of hero names to display in the training drop-downs
        /// </summary>
        private List<string> heroNames;

        /// <summary>
        /// The action to call when the bootcamp is fully resolved and the player attempts to exit.
        /// </summary>
        private Action<object, EventArgs> onEndFunc = null;
        #endregion

        #region Private methods
        /// <summary>
        /// Checks if all selections have been properly made. Returns true or false to correspond with the values found.
        /// </summary>
        /// <returns></returns>
        private bool checkButtonState()
        {
            for (int i = 0; i < contentTable.RowCount; i++)
            {
                int trainingMethod = -1;
                Control curPanel = contentTable.GetControlFromPosition(1, i);
                foreach (Control c in curPanel.Controls)
                {
                    if (c.Name.Equals("selection"))
                    {
                        if (((ComboBox)c).SelectedIndex == -1)
                        {
                            return false;
                        }
                        else
                        {
                            trainingMethod = ((ComboBox)c).SelectedIndex;
                        }
                    }
                    else if (c.Name.Equals("clarification"))
                    {
                        if (trainingMethod == 0)
                        {
                            if (((ComboBox)c).SelectedIndex == -1)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Submits all selections to the bootcamp session if all selections have been made.
        /// </summary>
        private void submitTrainingModes()
        {
            if (!checkButtonState())
            {
                return;
            }

            acceptButton.Enabled = false;
            acceptButton.Text = "...";

            for (int i = 0; i < contentTable.RowCount; i++)
            {
                int trainingMethod = -1;
                int clarification = -1;
                Control curPanel = contentTable.GetControlFromPosition(1, i);
                foreach (Control c in curPanel.Controls)
                {
                    if (c.Name.Equals("selection"))
                    {
                        ((ComboBox)c).Enabled = false;
                        trainingMethod = ((ComboBox)c).SelectedIndex;
                    }
                    else if (c.Name.Equals("clarification"))
                    {
                        ((ComboBox)c).Enabled = false;
                        clarification = ((ComboBox)c).SelectedIndex;                                
                    }
                }
                bm.SubmitTrainingMode(i, trainingMethod, clarification);
            }

            Task.Run(() => bm.InstantlyResolve());

            //Replace dropdowns with confirmation icon.
            for (int i = 0; i < contentTable.RowCount; i++)
            {
                Control curPanel = contentTable.GetControlFromPosition(1, i);
                curPanel.Hide();
                contentTable.Controls.Remove(curPanel);

                PictureBox confirm = new PictureBox();
                confirm.SizeMode = PictureBoxSizeMode.CenterImage;
                confirm.Image = new Bitmap(Properties.Resources.confirmation);
                confirm.Size = new Size(curPanel.Size.Width, Properties.Resources.confirmation.Size.Height);
                contentTable.Controls.Add(confirm, 1, i);
            }

            hasTrained = true;
            acceptButton.Enabled = true;
            acceptButton.Text = "Return";
        }

        /// <summary>
        /// Initializes the content table by creating and displaying the names of each player and drop-downs showing all of the options for training each player.
        /// </summary>
        private void initBootcampTable()
        {
            float height = 0;

            //Ensure table has enough slots.

            //Iterate through players and rows.
            for (int i = 0; i < contentTable.RowCount; i++)
            {
                string cur = bm.GetPlayerNames()[i];

                //Init player selection box
                ComboBox playerSelection = new ComboBox();
                playerSelection.FormattingEnabled = true;
                playerSelection.Size = new System.Drawing.Size(290, 21);
                playerSelection.Margin = new Padding(3);
                playerSelection.Location = new Point(3, 3);
                playerSelection.Name = "selection";
                playerSelection.Items.Add("Practice a specific hero.");
                playerSelection.Items.Add("Practice working with your team.");
                playerSelection.Items.Add("Practice your fingerwork.");

                //Init clarification selection box
                ComboBox clarificationSelection = new ComboBox();
                clarificationSelection.FormattingEnabled = true;
                clarificationSelection.Size = new Size(290, 21);
                clarificationSelection.Margin = new Padding(3);
                clarificationSelection.Location = new Point(3, 27);
                clarificationSelection.Name = "clarification";

                //Init selection actions
                Action<object, EventArgs> requireClarifiction = (object sender, EventArgs e) =>
                {
                    if (playerSelection.SelectedIndex == 0)
                    {
                        clarificationSelection.Items.Clear();
                        foreach (String s in heroNames)
                        {
                            clarificationSelection.Items.Add(s);
                        }
                        clarificationSelection.Show();
                    }
                    else
                    {
                        clarificationSelection.Items.Clear();
                        clarificationSelection.Hide();
                    }
                    acceptButton.Enabled = checkButtonState();
                };
                playerSelection.SelectedIndexChanged += new EventHandler(requireClarifiction);

                Action<object, EventArgs> check = (object sender, EventArgs e) =>
                {
                    acceptButton.Enabled = checkButtonState();
                };
                clarificationSelection.SelectedIndexChanged += new EventHandler(check);

                //Init containing panel
                Panel selectionContainer = new Panel();
                selectionContainer.Size = new Size(300, 60);
                selectionContainer.Controls.Add(playerSelection);
                selectionContainer.Controls.Add(clarificationSelection);

                //Add styling
                contentTable.RowStyles[i].Height = 60;
                contentTable.Controls.Add(selectionContainer, 1, i);
                clarificationSelection.Hide();

                //Add label for row
                Label playerTitle = new Label();
                playerTitle.Size = new Size(294, (int)contentTable.RowStyles[i].Height - 6);
                playerTitle.Margin = new Padding(3);
                playerTitle.Text = cur;
                playerTitle.Font = new Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                playerTitle.TextAlign = ContentAlignment.MiddleCenter;
                contentTable.Controls.Add(playerTitle, 0, i);

                height += contentTable.RowStyles[i].Height;
            }

            contentTable.Height = (int)Math.Round(height+1);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new bootcamp resolution control.
        /// </summary>
        /// <param name="bm">The bootcamp that the resolution control controls.</param>
        /// <param name="onEndFunc">The function to call when the control should be closed.</param>
        public BootcampResolutionControl(BootcampSession bm, Action<object, EventArgs> onEndFunc)
        {
            InitializeComponent();
            this.bm = bm;
            this.onEndFunc = onEndFunc;
            heroNames = bm.GetHeroNames();
            acceptButton.Enabled = false;
            titleText.Text = bm.teamName + " Bootcamp";
            initBootcampTable();
            checkButtonState();
        }
        #endregion

        #region Events
        /// <summary>
        /// Called when the parent of the control changes. Resizes the control to match the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BootcampResolutionControl_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Size = Parent.ClientSize;
            }
        }

        /// <summary>
        /// Called when the control resizes. Centers the content table and button to the center of the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BootcampResolutionControl_Resize(object sender, EventArgs e)
        {
            contentTable.Location = new Point((outerPanel.Size.Width - contentTable.Size.Width) / 2, contentTable.Location.Y);
            acceptButton.Location = new Point((outerPanel.Size.Width - acceptButton.Size.Width) / 2, acceptButton.Location.Y);
        }

        /// <summary>
        /// Called when the accept button is clicked. If training has not been completed, this finalizes training.
        /// If training is completed, closes the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acceptButton_Click(object sender, EventArgs e)
        {
            if (hasTrained)
            {
                onEndFunc?.Invoke(sender, e);
            }
            else
            {
                submitTrainingModes();
            }
        }
        #endregion
    }
}