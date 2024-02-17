namespace CodingTracker.View
{
    partial class MainPage
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            codingSessionMainButton = new Button();
            viewSessionButton = new Button();
            MainPageExitButton = new Button();
            SuspendLayout();
            // 
            // codingSessionMainButton
            // 
            codingSessionMainButton.Location = new Point(801, 242);
            codingSessionMainButton.Name = "codingSessionMainButton";
            codingSessionMainButton.Size = new Size(386, 49);
            codingSessionMainButton.TabIndex = 0;
            codingSessionMainButton.Text = "Coding session";
            codingSessionMainButton.UseVisualStyleBackColor = true;
            codingSessionMainButton.Click += button1_Click;
            // 
            // viewSessionButton
            // 
            viewSessionButton.Location = new Point(801, 343);
            viewSessionButton.Name = "viewSessionButton";
            viewSessionButton.Size = new Size(386, 54);
            viewSessionButton.TabIndex = 1;
            viewSessionButton.Text = "View coding sessions";
            viewSessionButton.UseVisualStyleBackColor = true;
            // 
            // MainPageExitButton
            // 
            MainPageExitButton.Location = new Point(1997, 678);
            MainPageExitButton.Name = "MainPageExitButton";
            MainPageExitButton.Size = new Size(156, 60);
            MainPageExitButton.TabIndex = 2;
            MainPageExitButton.Text = "Exit";
            MainPageExitButton.UseVisualStyleBackColor = true;
            // 
            // MainPage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2147, 750);
            Controls.Add(MainPageExitButton);
            Controls.Add(viewSessionButton);
            Controls.Add(codingSessionMainButton);
            Name = "MainPage";
            Text = "MainPage";
            ResumeLayout(false);
        }

        #endregion

        private Button codingSessionMainButton;
        private Button viewSessionButton;
        private Button MainPageExitButton;
    }
}