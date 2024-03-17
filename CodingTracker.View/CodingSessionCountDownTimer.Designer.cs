namespace CodingTracker.View
{
    partial class CodingSessionCountDownTimer
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
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            timer1 = new System.Windows.Forms.Timer(components);
            CodingSessionTimerPageProgressBar = new Guna.UI2.WinForms.Guna2ProgressBar();
            CodingSessionTimerPageTimerLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            CodingTimerPageExitButton = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Interval = 1000;
            // 
            // CodingSessionTimerPageProgressBar
            // 
            CodingSessionTimerPageProgressBar.CustomizableEdges = customizableEdges5;
            CodingSessionTimerPageProgressBar.Location = new Point(460, 338);
            CodingSessionTimerPageProgressBar.Name = "CodingSessionTimerPageProgressBar";
            CodingSessionTimerPageProgressBar.ProgressColor = Color.FromArgb(128, 255, 128);
            CodingSessionTimerPageProgressBar.ProgressColor2 = Color.Green;
            CodingSessionTimerPageProgressBar.ShadowDecoration.CustomizableEdges = customizableEdges6;
            CodingSessionTimerPageProgressBar.Size = new Size(300, 30);
            CodingSessionTimerPageProgressBar.TabIndex = 0;
            CodingSessionTimerPageProgressBar.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            CodingSessionTimerPageProgressBar.Value = 50;
            // 
            // CodingSessionTimerPageTimerLabel
            // 
            CodingSessionTimerPageTimerLabel.AutoSize = false;
            CodingSessionTimerPageTimerLabel.BackColor = Color.Transparent;
            CodingSessionTimerPageTimerLabel.IsSelectionEnabled = false;
            CodingSessionTimerPageTimerLabel.Location = new Point(537, 193);
            CodingSessionTimerPageTimerLabel.Name = "CodingSessionTimerPageTimerLabel";
            CodingSessionTimerPageTimerLabel.Size = new Size(110, 43);
            CodingSessionTimerPageTimerLabel.TabIndex = 1;
            CodingSessionTimerPageTimerLabel.Text = "00:00:00";
            CodingSessionTimerPageTimerLabel.TextAlignment = ContentAlignment.MiddleCenter;
            CodingSessionTimerPageTimerLabel.Click += CodingSessionTimerPageTimerLabel_Click;
            // 
            // CodingTimerPageExitButton
            // 
            CodingTimerPageExitButton.CustomizableEdges = customizableEdges7;
            CodingTimerPageExitButton.DisabledState.BorderColor = Color.DarkGray;
            CodingTimerPageExitButton.DisabledState.CustomBorderColor = Color.DarkGray;
            CodingTimerPageExitButton.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            CodingTimerPageExitButton.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            CodingTimerPageExitButton.Font = new Font("Segoe UI", 9F);
            CodingTimerPageExitButton.ForeColor = Color.White;
            CodingTimerPageExitButton.Location = new Point(1211, 0);
            CodingTimerPageExitButton.Name = "CodingTimerPageExitButton";
            CodingTimerPageExitButton.ShadowDecoration.CustomizableEdges = customizableEdges8;
            CodingTimerPageExitButton.Size = new Size(73, 45);
            CodingTimerPageExitButton.TabIndex = 6;
            CodingTimerPageExitButton.Text = "Exit";
            // 
            // CodingSessionTimer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1284, 681);
            Controls.Add(CodingTimerPageExitButton);
            Controls.Add(CodingSessionTimerPageTimerLabel);
            Controls.Add(CodingSessionTimerPageProgressBar);
            Name = "CodingSessionTimer";
            Text = "CodingSessionTimer";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private Guna.UI2.WinForms.Guna2ProgressBar CodingSessionTimerPageProgressBar;
        private Guna.UI2.WinForms.Guna2HtmlLabel CodingSessionTimerPageTimerLabel;
        private Guna.UI2.WinForms.Guna2Button CodingTimerPageExitButton;
    }
}