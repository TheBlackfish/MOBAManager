namespace MOBAManager.UI
{
    partial class MainMenuControl
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
            this.newGameButton = new System.Windows.Forms.Button();
            this.continueButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.centerPanel = new System.Windows.Forms.Panel();
            this.centerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // newGameButton
            // 
            this.newGameButton.Location = new System.Drawing.Point(3, 3);
            this.newGameButton.Name = "newGameButton";
            this.newGameButton.Size = new System.Drawing.Size(394, 40);
            this.newGameButton.TabIndex = 0;
            this.newGameButton.Text = "New Game";
            this.newGameButton.UseVisualStyleBackColor = true;
            this.newGameButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // continueButton
            // 
            this.continueButton.Location = new System.Drawing.Point(3, 49);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(394, 40);
            this.continueButton.TabIndex = 1;
            this.continueButton.Text = "Continue";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // exitButton
            // 
            this.exitButton.Location = new System.Drawing.Point(3, 95);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(394, 40);
            this.exitButton.TabIndex = 2;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.button3_Click);
            // 
            // centerPanel
            // 
            this.centerPanel.Controls.Add(this.newGameButton);
            this.centerPanel.Controls.Add(this.exitButton);
            this.centerPanel.Controls.Add(this.continueButton);
            this.centerPanel.Location = new System.Drawing.Point(400, 256);
            this.centerPanel.Name = "centerPanel";
            this.centerPanel.Size = new System.Drawing.Size(400, 138);
            this.centerPanel.TabIndex = 3;
            // 
            // MainMenuControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.centerPanel);
            this.Name = "MainMenuControl";
            this.Size = new System.Drawing.Size(1146, 641);
            this.ParentChanged += new System.EventHandler(this.MainMenuControl_ParentChanged);
            this.centerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button newGameButton;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Panel centerPanel;
    }
}
