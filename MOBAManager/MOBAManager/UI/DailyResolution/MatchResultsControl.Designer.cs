﻿namespace MOBAManager.UI
{
    partial class MatchResultsControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.team2Info = new System.Windows.Forms.RichTextBox();
            this.team1Info = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.winningTeamLabel = new System.Windows.Forms.Label();
            this.continueButton = new System.Windows.Forms.Button();
            this.centerPanel = new System.Windows.Forms.Panel();
            this.centerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // team2Info
            // 
            this.team2Info.BackColor = System.Drawing.SystemColors.Control;
            this.team2Info.Cursor = System.Windows.Forms.Cursors.Default;
            this.team2Info.Location = new System.Drawing.Point(247, 105);
            this.team2Info.Name = "team2Info";
            this.team2Info.ReadOnly = true;
            this.team2Info.ShortcutsEnabled = false;
            this.team2Info.Size = new System.Drawing.Size(229, 110);
            this.team2Info.TabIndex = 7;
            this.team2Info.Text = "Example Team\n-----\nBritishname Complicated - Charles Dexter Ward\nUsername\nUsernam" +
    "e\nUsername\nUsername";
            // 
            // team1Info
            // 
            this.team1Info.Cursor = System.Windows.Forms.Cursors.Default;
            this.team1Info.Location = new System.Drawing.Point(2, 105);
            this.team1Info.Name = "team1Info";
            this.team1Info.ReadOnly = true;
            this.team1Info.ShortcutsEnabled = false;
            this.team1Info.Size = new System.Drawing.Size(229, 110);
            this.team1Info.TabIndex = 8;
            this.team1Info.Text = "Example Team\n-----\nBritishname Complicated - Charles Dexter Ward\nUsername\nUsernam" +
    "e\nUsername\nUsername";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(162, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 37);
            this.label1.TabIndex = 9;
            this.label1.Text = "WINNER";
            // 
            // winningTeamLabel
            // 
            this.winningTeamLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.winningTeamLabel.Location = new System.Drawing.Point(2, 42);
            this.winningTeamLabel.Name = "winningTeamLabel";
            this.winningTeamLabel.Size = new System.Drawing.Size(474, 42);
            this.winningTeamLabel.TabIndex = 10;
            this.winningTeamLabel.Text = "WINNING TEAM";
            this.winningTeamLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // continueButton
            // 
            this.continueButton.Location = new System.Drawing.Point(401, 221);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(75, 23);
            this.continueButton.TabIndex = 11;
            this.continueButton.Text = "Continue";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // centerPanel
            // 
            this.centerPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.centerPanel.Controls.Add(this.team2Info);
            this.centerPanel.Controls.Add(this.continueButton);
            this.centerPanel.Controls.Add(this.team1Info);
            this.centerPanel.Controls.Add(this.winningTeamLabel);
            this.centerPanel.Controls.Add(this.label1);
            this.centerPanel.Location = new System.Drawing.Point(3, 3);
            this.centerPanel.Name = "centerPanel";
            this.centerPanel.Size = new System.Drawing.Size(478, 246);
            this.centerPanel.TabIndex = 12;
            // 
            // MatchResultsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.centerPanel);
            this.Name = "MatchResultsControl";
            this.Size = new System.Drawing.Size(484, 252);
            this.ParentChanged += new System.EventHandler(this.MatchResultsControl_ParentChanged);
            this.centerPanel.ResumeLayout(false);
            this.centerPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RichTextBox team2Info;
        private System.Windows.Forms.RichTextBox team1Info;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label winningTeamLabel;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.Panel centerPanel;
    }
}
