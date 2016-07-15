namespace MOBAManager.UI
{
    partial class DailyMenu
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
            this.label1 = new System.Windows.Forms.Label();
            this.teamButton = new System.Windows.Forms.Button();
            this.calendarButton = new System.Windows.Forms.Button();
            this.metaButton = new System.Windows.Forms.Button();
            this.resolutionButton = new System.Windows.Forms.Button();
            this.dailySummaryArea = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1034, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Example Date";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // teamButton
            // 
            this.teamButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.teamButton.Location = new System.Drawing.Point(3, 47);
            this.teamButton.Name = "teamButton";
            this.teamButton.Size = new System.Drawing.Size(1034, 50);
            this.teamButton.TabIndex = 1;
            this.teamButton.Text = "View Team (In Progress)";
            this.teamButton.UseVisualStyleBackColor = true;
            // 
            // calendarButton
            // 
            this.calendarButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.calendarButton.Location = new System.Drawing.Point(3, 103);
            this.calendarButton.Name = "calendarButton";
            this.calendarButton.Size = new System.Drawing.Size(1034, 50);
            this.calendarButton.TabIndex = 2;
            this.calendarButton.Text = "View Calendar (In Progress)";
            this.calendarButton.UseVisualStyleBackColor = true;
            // 
            // metaButton
            // 
            this.metaButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metaButton.Location = new System.Drawing.Point(3, 159);
            this.metaButton.Name = "metaButton";
            this.metaButton.Size = new System.Drawing.Size(1034, 50);
            this.metaButton.TabIndex = 3;
            this.metaButton.Text = "View Meta (In Progress)";
            this.metaButton.UseVisualStyleBackColor = true;
            this.metaButton.Click += new System.EventHandler(this.metaButton_Click);
            // 
            // resolutionButton
            // 
            this.resolutionButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resolutionButton.Location = new System.Drawing.Point(3, 337);
            this.resolutionButton.Name = "resolutionButton";
            this.resolutionButton.Size = new System.Drawing.Size(1034, 50);
            this.resolutionButton.TabIndex = 4;
            this.resolutionButton.Text = "Resolve Today\'s Events";
            this.resolutionButton.UseVisualStyleBackColor = true;
            this.resolutionButton.Click += new System.EventHandler(this.resolutionButton_Click);
            // 
            // dailySummaryArea
            // 
            this.dailySummaryArea.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dailySummaryArea.Location = new System.Drawing.Point(3, 215);
            this.dailySummaryArea.Multiline = true;
            this.dailySummaryArea.Name = "dailySummaryArea";
            this.dailySummaryArea.ReadOnly = true;
            this.dailySummaryArea.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dailySummaryArea.Size = new System.Drawing.Size(1034, 116);
            this.dailySummaryArea.TabIndex = 5;
            this.dailySummaryArea.Text = "Daily Summary goes here.";
            // 
            // DailyMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dailySummaryArea);
            this.Controls.Add(this.resolutionButton);
            this.Controls.Add(this.metaButton);
            this.Controls.Add(this.calendarButton);
            this.Controls.Add(this.teamButton);
            this.Controls.Add(this.label1);
            this.Name = "DailyMenu";
            this.Size = new System.Drawing.Size(1040, 391);
            this.ParentChanged += new System.EventHandler(this.DailyMenu_ParentChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button teamButton;
        private System.Windows.Forms.Button calendarButton;
        private System.Windows.Forms.Button metaButton;
        private System.Windows.Forms.Button resolutionButton;
        private System.Windows.Forms.TextBox dailySummaryArea;
    }
}
