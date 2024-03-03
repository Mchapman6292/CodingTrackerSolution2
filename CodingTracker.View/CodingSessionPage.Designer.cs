using CodingTracker.Business.CodingSession;

namespace CodingTracker.View
{
    partial class CodingSessionPage
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


        private void StartSessionButton_Click(object sender, EventArgs e)
        {
            try
            {
                _codingSession.StartSession();
                MessageBox.Show("Session started.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void EndSessionButton_Click(object sender, EventArgs e)
        {
            try
            {
                _codingSession.EndSession();
                MessageBox.Show("Session ended.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ViewSessionsButton_Click(object sender, EventArgs e)
        {
            try
            {


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void SetGoalButton_Click(object sender, EventArgs e)
        {
            try
            {

                int goalHours = _utilityService.TryParseInt(goalHoursTextBox.Text);
                _codingSession.SetCodingGoal(goalHours);
                MessageBox.Show("Coding goal set.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            startCodingSessionButton = new Button();
            enterStartTimeButton = new Button();
            enterEndTimeButton = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            checkBox1 = new CheckBox();
            SuspendLayout();
            // 
            // startCodingSessionButton
            // 
            startCodingSessionButton.Location = new Point(671, 233);
            startCodingSessionButton.Name = "startCodingSessionButton";
            startCodingSessionButton.Size = new Size(493, 80);
            startCodingSessionButton.TabIndex = 0;
            startCodingSessionButton.Text = "Start coding session";
            startCodingSessionButton.UseVisualStyleBackColor = true;
            startCodingSessionButton.Click += StartCodingSessionButton_Click;
            // 
            // enterStartTimeButton
            // 
            enterStartTimeButton.Location = new Point(131, 483);
            enterStartTimeButton.Name = "enterStartTimeButton";
            enterStartTimeButton.Size = new Size(496, 77);
            enterStartTimeButton.TabIndex = 1;
            enterStartTimeButton.Text = "Enter start time";
            enterStartTimeButton.UseVisualStyleBackColor = true;
            enterStartTimeButton.Click += EnterStartTimeButton_Click;
            // 
            // enterEndTimeButton
            // 
            enterEndTimeButton.Location = new Point(1184, 483);
            enterEndTimeButton.Name = "enterEndTimeButton";
            enterEndTimeButton.Size = new Size(496, 88);
            enterEndTimeButton.TabIndex = 2;
            enterEndTimeButton.Text = "Enter end time";
            enterEndTimeButton.UseVisualStyleBackColor = true;
            enterEndTimeButton.Click += EnterEndTimeButton_Click;
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // checkBox1
            // 
            checkBox1.Location = new Point(1402, 233);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(197, 80);
            checkBox1.TabIndex = 3;
            checkBox1.Text = "Show Stopwatch";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // CodingSessionPage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2469, 715);
            Controls.Add(checkBox1);
            Controls.Add(enterEndTimeButton);
            Controls.Add(enterStartTimeButton);
            Controls.Add(startCodingSessionButton);
            Name = "CodingSessionPage";
            Text = "CodingSessionPage";
            ResumeLayout(false);
        }

        #endregion

        private Button startCodingSessionButton;
        private Button enterStartTimeButton;
        private Button enterEndTimeButton;
        private System.Windows.Forms.Timer timer1;
        private CheckBox checkBox1;
    }
}