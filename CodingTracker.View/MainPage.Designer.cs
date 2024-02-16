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
            StartSessionButton = new Button();
            ViewSessionButton = new Button();
            MainPageExitButton = new Button();
            SuspendLayout();
            // 
            // StartSessionButton
            // 
            StartSessionButton.Location = new Point(801, 242);
            StartSessionButton.Name = "StartSessionButton";
            StartSessionButton.Size = new Size(386, 49);
            StartSessionButton.TabIndex = 0;
            StartSessionButton.Text = "Start Coding Session";
            StartSessionButton.UseVisualStyleBackColor = true;
            StartSessionButton.Click += button1_Click;
            // 
            // ViewSessionButton
            // 
            ViewSessionButton.Location = new Point(801, 343);
            ViewSessionButton.Name = "ViewSessionButton";
            ViewSessionButton.Size = new Size(386, 54);
            ViewSessionButton.TabIndex = 1;
            ViewSessionButton.Text = "View Coding Sessions";
            ViewSessionButton.UseVisualStyleBackColor = true;
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
            Controls.Add(ViewSessionButton);
            Controls.Add(StartSessionButton);
            Name = "MainPage";
            Text = "MainPage";
            ResumeLayout(false);
        }

        #endregion

        private Button StartSessionButton;
        private Button ViewSessionButton;
        private Button MainPageExitButton;
    }
}