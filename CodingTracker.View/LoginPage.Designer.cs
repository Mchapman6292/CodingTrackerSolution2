using System;
using System.Linq.Expressions;
using System.Windows.Forms;
using CodingTracker.Business.CodingSession;
using CodingTracker.Logging.ICRUDs;
using CodingTracker.Logging.IDatabaseManagers;
using CodingTracker.Logging.IInputValidators;
using CodingTracker.Logging.IUtilityServices;
using CodingTracker.Logging.UtilityServices;

namespace CodingTracker.View
{
    partial class loginPageForm
    {
        private readonly ICRUD _crud;
        private readonly IDatabaseManager _dbManager;
        private readonly IInputValidator _inputValidator;
        private readonly IUtilityService _utilityService;
        private readonly CodingSession _codingSession;
        private Button startSessionButton;
        private Button endSessionButton;
        private Button viewSessionsButton;
        private Button setGoalButton;
        private TextBox goalHoursTextBox;
        private DataGridView sessionsDataGridView;
        private bool _isSessionActive = false;
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
            loginPageUsernameLabel = new Label();
            Password = new Label();
            loginPagePasswordTextbox = new TextBox();
            loginPageUsernameTextbox = new TextBox();
            loginPageErrorTextbox = new Label();
            loginPageLoginButton = new Button();
            loginPageExitButton = new Button();
            SuspendLayout();
            // 
            // loginPageUsernameLabel
            // 
            loginPageUsernameLabel.Location = new Point(631, 207);
            loginPageUsernameLabel.Name = "loginPageUsernameLabel";
            loginPageUsernameLabel.Size = new Size(100, 23);
            loginPageUsernameLabel.TabIndex = 0;
            loginPageUsernameLabel.Text = "Username";
            // 
            // Password
            // 
            Password.Location = new Point(629, 270);
            Password.Name = "Password";
            Password.Size = new Size(100, 23);
            Password.TabIndex = 1;
            Password.Text = "Password";
            // 
            // loginPagePasswordTextbox
            // 
            loginPagePasswordTextbox.AcceptsReturn = true;
            loginPagePasswordTextbox.CausesValidation = false;
            loginPagePasswordTextbox.Location = new Point(812, 270);
            loginPagePasswordTextbox.Name = "loginPagePasswordTextbox";
            loginPagePasswordTextbox.Size = new Size(223, 23);
            loginPagePasswordTextbox.TabIndex = 2;
            // 
            // loginPageUsernameTextbox
            // 
            loginPageUsernameTextbox.Location = new Point(812, 207);
            loginPageUsernameTextbox.Name = "loginPageUsernameTextbox";
            loginPageUsernameTextbox.Size = new Size(223, 23);
            loginPageUsernameTextbox.TabIndex = 3;
            // 
            // loginPageErrorTextbox
            // 
            loginPageErrorTextbox.Location = new Point(809, 425);
            loginPageErrorTextbox.Name = "loginPageErrorTextbox";
            loginPageErrorTextbox.Size = new Size(226, 23);
            loginPageErrorTextbox.TabIndex = 5;
            loginPageErrorTextbox.Visible = false;
            // 
            // loginPageLoginButton
            // 
            loginPageLoginButton.Location = new Point(812, 349);
            loginPageLoginButton.Name = "loginPageLoginButton";
            loginPageLoginButton.Size = new Size(223, 33);
            loginPageLoginButton.TabIndex = 6;
            loginPageLoginButton.Text = "Login";
            loginPageLoginButton.UseVisualStyleBackColor = true;
            loginPageLoginButton.Click += loginPageLoginButton_Click;
            // 
            // loginPageExitButton
            // 
            loginPageExitButton.Location = new Point(1215, 425);
            loginPageExitButton.Name = "loginPageExitButton";
            loginPageExitButton.Size = new Size(90, 41);
            loginPageExitButton.TabIndex = 8;
            loginPageExitButton.Text = "Exit";
            loginPageExitButton.UseVisualStyleBackColor = true;
            loginPageExitButton.Click += loginPageExitButton_Click;
            // 
            // loginPageForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2481, 584);
            Controls.Add(loginPageExitButton);
            Controls.Add(loginPageLoginButton);
            Controls.Add(loginPageErrorTextbox);
            Controls.Add(loginPageUsernameTextbox);
            Controls.Add(loginPagePasswordTextbox);
            Controls.Add(Password);
            Controls.Add(loginPageUsernameLabel);
            Name = "loginPageForm";
            ResumeLayout(false);
            PerformLayout();
        }

        private void StartSessionButton_Click(object sender, EventArgs e)
        {
            if (!_isSessionActive)
            {
                try
                {
                    _codingSession.StartSession();
                    _isSessionActive = true;
                    startSessionButton.Enabled = false;
                    MessageBox.Show("Session started.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void EndSessionButton_Click(object sender, EventArgs e)
        {
            if (_isSessionActive)
            {
                try
                {
                    _codingSession.EndSession();
                    _isSessionActive = false;
                    endSessionButton.Enabled = true;
                    MessageBox.Show("Session ended.");
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
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
        #endregion

        private Label loginPageUsernameLabel;
        private Label Password;
        private TextBox loginPagePasswordTextbox;
        private TextBox loginPageUsernameTextbox;
        private Label loginPageErrorTextbox;
        private Button loginPageLoginButton;
        private Button button1;
        private Button loginPageExitButton;
    }
}
