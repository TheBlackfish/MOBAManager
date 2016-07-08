namespace MOBAManager.UI
{
    partial class EventResolutionControl
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
            this.titleText = new System.Windows.Forms.Label();
            this.eventContainer = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // titleText
            // 
            this.titleText.AutoSize = true;
            this.titleText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleText.Location = new System.Drawing.Point(3, 3);
            this.titleText.Name = "titleText";
            this.titleText.Size = new System.Drawing.Size(158, 25);
            this.titleText.TabIndex = 0;
            this.titleText.Text = "Example Date";
            // 
            // eventContainer
            // 
            this.eventContainer.AutoScroll = true;
            this.eventContainer.Location = new System.Drawing.Point(3, 32);
            this.eventContainer.Name = "eventContainer";
            this.eventContainer.Size = new System.Drawing.Size(694, 565);
            this.eventContainer.TabIndex = 1;
            // 
            // EventResolutionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.eventContainer);
            this.Controls.Add(this.titleText);
            this.Name = "EventResolutionControl";
            this.Size = new System.Drawing.Size(700, 600);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label titleText;
        private System.Windows.Forms.Panel eventContainer;
    }
}
