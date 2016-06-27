namespace MOBAManager.UI
{
    partial class MatchResolutionControl
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
            this.components = new System.ComponentModel.Container();
            this.team1TimerA = new System.Windows.Forms.TextBox();
            this.team1TimerB = new System.Windows.Forms.TextBox();
            this.team1Info = new System.Windows.Forms.RichTextBox();
            this.team1Bans = new System.Windows.Forms.RichTextBox();
            this.team1Picks = new System.Windows.Forms.RichTextBox();
            this.team2Picks = new System.Windows.Forms.RichTextBox();
            this.team2Bans = new System.Windows.Forms.RichTextBox();
            this.team2Info = new System.Windows.Forms.RichTextBox();
            this.team2TimerB = new System.Windows.Forms.TextBox();
            this.team2TimerA = new System.Windows.Forms.TextBox();
            this.userSelection = new System.Windows.Forms.ComboBox();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // team1TimerA
            // 
            this.team1TimerA.Location = new System.Drawing.Point(3, 3);
            this.team1TimerA.Multiline = true;
            this.team1TimerA.Name = "team1TimerA";
            this.team1TimerA.ReadOnly = true;
            this.team1TimerA.Size = new System.Drawing.Size(32, 20);
            this.team1TimerA.TabIndex = 0;
            this.team1TimerA.Text = "0:00";
            // 
            // team1TimerB
            // 
            this.team1TimerB.Location = new System.Drawing.Point(3, 30);
            this.team1TimerB.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.team1TimerB.Multiline = true;
            this.team1TimerB.Name = "team1TimerB";
            this.team1TimerB.ReadOnly = true;
            this.team1TimerB.Size = new System.Drawing.Size(32, 20);
            this.team1TimerB.TabIndex = 1;
            this.team1TimerB.Text = "0:00";
            // 
            // team1Info
            // 
            this.team1Info.Location = new System.Drawing.Point(41, 3);
            this.team1Info.Name = "team1Info";
            this.team1Info.ReadOnly = true;
            this.team1Info.ShortcutsEnabled = false;
            this.team1Info.Size = new System.Drawing.Size(140, 110);
            this.team1Info.TabIndex = 2;
            this.team1Info.Text = "Example Team\n-----\nBritishname Complicated\nUsername\nUsername\nUsername\nUsername";
            // 
            // team1Bans
            // 
            this.team1Bans.Location = new System.Drawing.Point(41, 119);
            this.team1Bans.Name = "team1Bans";
            this.team1Bans.ReadOnly = true;
            this.team1Bans.ShortcutsEnabled = false;
            this.team1Bans.Size = new System.Drawing.Size(140, 75);
            this.team1Bans.TabIndex = 3;
            this.team1Bans.Text = "Ban 1\nBan 2\nBan 3\nBan 4\nBan 5";
            // 
            // team1Picks
            // 
            this.team1Picks.Location = new System.Drawing.Point(41, 200);
            this.team1Picks.Name = "team1Picks";
            this.team1Picks.ReadOnly = true;
            this.team1Picks.ShortcutsEnabled = false;
            this.team1Picks.Size = new System.Drawing.Size(140, 75);
            this.team1Picks.TabIndex = 4;
            this.team1Picks.Text = "Ban 1\nBan 2\nBan 3\nBan 4\nBan 5";
            // 
            // team2Picks
            // 
            this.team2Picks.Location = new System.Drawing.Point(187, 200);
            this.team2Picks.Name = "team2Picks";
            this.team2Picks.ReadOnly = true;
            this.team2Picks.ShortcutsEnabled = false;
            this.team2Picks.Size = new System.Drawing.Size(140, 75);
            this.team2Picks.TabIndex = 7;
            this.team2Picks.Text = "Ban 1\nBan 2\nBan 3\nBan 4\nBan 5";
            // 
            // team2Bans
            // 
            this.team2Bans.Location = new System.Drawing.Point(187, 119);
            this.team2Bans.Name = "team2Bans";
            this.team2Bans.ReadOnly = true;
            this.team2Bans.ShortcutsEnabled = false;
            this.team2Bans.Size = new System.Drawing.Size(140, 75);
            this.team2Bans.TabIndex = 6;
            this.team2Bans.Text = "Ban 1\nBan 2\nBan 3\nBan 4\nBan 5";
            // 
            // team2Info
            // 
            this.team2Info.Location = new System.Drawing.Point(187, 3);
            this.team2Info.Name = "team2Info";
            this.team2Info.ReadOnly = true;
            this.team2Info.ShortcutsEnabled = false;
            this.team2Info.Size = new System.Drawing.Size(140, 110);
            this.team2Info.TabIndex = 5;
            this.team2Info.Text = "Example Team\n-----\nBritishname Complicated\nUsername\nUsername\nUsername\nUsername";
            // 
            // team2TimerB
            // 
            this.team2TimerB.Location = new System.Drawing.Point(333, 30);
            this.team2TimerB.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.team2TimerB.Multiline = true;
            this.team2TimerB.Name = "team2TimerB";
            this.team2TimerB.ReadOnly = true;
            this.team2TimerB.Size = new System.Drawing.Size(32, 20);
            this.team2TimerB.TabIndex = 9;
            this.team2TimerB.Text = "0:00";
            // 
            // team2TimerA
            // 
            this.team2TimerA.Location = new System.Drawing.Point(333, 3);
            this.team2TimerA.Multiline = true;
            this.team2TimerA.Name = "team2TimerA";
            this.team2TimerA.ReadOnly = true;
            this.team2TimerA.Size = new System.Drawing.Size(32, 20);
            this.team2TimerA.TabIndex = 8;
            this.team2TimerA.Text = "0:00";
            // 
            // userSelection
            // 
            this.userSelection.FormattingEnabled = true;
            this.userSelection.Location = new System.Drawing.Point(3, 282);
            this.userSelection.Name = "userSelection";
            this.userSelection.Size = new System.Drawing.Size(374, 21);
            this.userSelection.TabIndex = 10;
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // MatchResolutionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.userSelection);
            this.Controls.Add(this.team2TimerB);
            this.Controls.Add(this.team2TimerA);
            this.Controls.Add(this.team2Picks);
            this.Controls.Add(this.team2Bans);
            this.Controls.Add(this.team2Info);
            this.Controls.Add(this.team1Picks);
            this.Controls.Add(this.team1Bans);
            this.Controls.Add(this.team1Info);
            this.Controls.Add(this.team1TimerB);
            this.Controls.Add(this.team1TimerA);
            this.Name = "MatchResolutionControl";
            this.Size = new System.Drawing.Size(380, 320);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox team1TimerA;
        private System.Windows.Forms.TextBox team1TimerB;
        private System.Windows.Forms.RichTextBox team1Info;
        private System.Windows.Forms.RichTextBox team1Bans;
        private System.Windows.Forms.RichTextBox team1Picks;
        private System.Windows.Forms.RichTextBox team2Picks;
        private System.Windows.Forms.RichTextBox team2Bans;
        private System.Windows.Forms.RichTextBox team2Info;
        private System.Windows.Forms.TextBox team2TimerB;
        private System.Windows.Forms.TextBox team2TimerA;
        private System.Windows.Forms.ComboBox userSelection;
        private System.Windows.Forms.Timer updateTimer;
    }
}
