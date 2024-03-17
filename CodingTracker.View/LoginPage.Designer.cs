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
    partial class LoginPage
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            loginPageUsernameLabel = new Label();
            Password = new Label();
            loginPagePasswordTextbox = new TextBox();
            loginPageUsernameTextbox = new TextBox();
            loginPageErrorTextbox = new Label();
            loginPageLoginButton = new Button();
            loginPageExitButton = new Button();
            guna2CustomCheckBox1 = new Guna.UI2.WinForms.Guna2CustomCheckBox();
            label1 = new Label();
            LoginPageForgotResetAccountButton = new Button();
            SuspendLayout();
            // 
            // loginPageUsernameLabel
            // 
            loginPageUsernameLabel.Location = new Point(514, 188);
            loginPageUsernameLabel.Name = "loginPageUsernameLabel";
            loginPageUsernameLabel.Size = new Size(100, 23);
            loginPageUsernameLabel.TabIndex = 0;
            loginPageUsernameLabel.Text = "Username";
            // 
            // Password
            // 
            Password.Location = new Point(514, 280);
            Password.Name = "Password";
            Password.Size = new Size(100, 23);
            Password.TabIndex = 1;
            Password.Text = "Password";
            // 
            // loginPagePasswordTextbox
            // 
            loginPagePasswordTextbox.AcceptsReturn = true;
            loginPagePasswordTextbox.CausesValidation = false;
            loginPagePasswordTextbox.Location = new Point(514, 306);
            loginPagePasswordTextbox.Multiline = true;
            loginPagePasswordTextbox.Name = "loginPagePasswordTextbox";
            loginPagePasswordTextbox.Size = new Size(235, 33);
            loginPagePasswordTextbox.TabIndex = 2;
            // 
            // loginPageUsernameTextbox
            // 
            loginPageUsernameTextbox.Location = new Point(514, 214);
            loginPageUsernameTextbox.Multiline = true;
            loginPageUsernameTextbox.Name = "loginPageUsernameTextbox";
            loginPageUsernameTextbox.Size = new Size(235, 33);
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
            loginPageLoginButton.Location = new Point(514, 410);
            loginPageLoginButton.Name = "loginPageLoginButton";
            loginPageLoginButton.Size = new Size(235, 38);
            loginPageLoginButton.TabIndex = 6;
            loginPageLoginButton.Text = "Login";
            loginPageLoginButton.UseVisualStyleBackColor = true;
            loginPageLoginButton.Click += loginPageLoginButton_Click;
            // 
            // loginPageExitButton
            // 
            loginPageExitButton.Location = new Point(1222, 1);
            loginPageExitButton.Name = "loginPageExitButton";
            loginPageExitButton.Size = new Size(62, 32);
            loginPageExitButton.TabIndex = 8;
            loginPageExitButton.Text = "Exit";
            loginPageExitButton.UseVisualStyleBackColor = true;
            loginPageExitButton.Click += loginPageExitButton_Click;
            // 
            // guna2CustomCheckBox1
            // 
            guna2CustomCheckBox1.BackColor = SystemColors.ControlLight;
            guna2CustomCheckBox1.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            guna2CustomCheckBox1.CheckedState.BorderRadius = 2;
            guna2CustomCheckBox1.CheckedState.BorderThickness = 0;
            guna2CustomCheckBox1.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            guna2CustomCheckBox1.CustomizableEdges = customizableEdges3;
            guna2CustomCheckBox1.Location = new Point(514, 361);
            guna2CustomCheckBox1.Name = "guna2CustomCheckBox1";
            guna2CustomCheckBox1.ShadowDecoration.Color = Color.Transparent;
            guna2CustomCheckBox1.ShadowDecoration.CustomizableEdges = customizableEdges4;
            guna2CustomCheckBox1.Size = new Size(20, 20);
            guna2CustomCheckBox1.TabIndex = 9;
            guna2CustomCheckBox1.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            guna2CustomCheckBox1.UncheckedState.BorderRadius = 2;
            guna2CustomCheckBox1.UncheckedState.BorderThickness = 0;
            guna2CustomCheckBox1.UncheckedState.FillColor = Color.FromArgb(125, 137, 149);
            // 
            // label1
            // 
            label1.Location = new Point(540, 361);
            label1.Name = "label1";
            label1.Size = new Size(100, 23);
            label1.TabIndex = 10;
            label1.Text = "Remember me";
            // 
            // LoginPageForgotResetAccountButton
            // 
            LoginPageForgotResetAccountButton.Location = new Point(1049, 641);
            LoginPageForgotResetAccountButton.Name = "LoginPageForgotResetAccountButton";
            LoginPageForgotResetAccountButton.Size = new Size(235, 38);
            LoginPageForgotResetAccountButton.TabIndex = 11;
            LoginPageForgotResetAccountButton.Text = "Reset account";
            LoginPageForgotResetAccountButton.UseVisualStyleBackColor = true;
            // 
            // LoginPage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1284, 681);
            Controls.Add(LoginPageForgotResetAccountButton);
            Controls.Add(label1);
            Controls.Add(guna2CustomCheckBox1);
            Controls.Add(loginPageExitButton);
            Controls.Add(loginPageLoginButton);
            Controls.Add(loginPageErrorTextbox);
            Controls.Add(loginPageUsernameTextbox);
            Controls.Add(loginPagePasswordTextbox);
            Controls.Add(Password);
            Controls.Add(loginPageUsernameLabel);
            Name = "LoginPage";
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
        private Button loginPageExitButton;
        private Guna.UI2.WinForms.Guna2CustomCheckBox guna2CustomCheckBox1;
        private Label label1;
        private Button LoginPageForgotResetAccountButton;
    }
}
