using CodingTracker.Business.CodingSessions;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.IUtilityServices;

namespace CodingTracker.View
{
    partial class LoginPage
    {
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginPage));
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            loginPageErrorTextbox = new Label();
            label1 = new Label();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            LoginPageVLCPLayer = new LibVLCSharp.WinForms.VideoView();
            loginPageUsernameTextbox = new Guna.UI2.WinForms.Guna2TextBox();
            LoginPagePasswordTextbox = new Guna.UI2.WinForms.Guna2TextBox();
            LoginPageRememberMeToggle = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            loginPageLoginButton = new Guna.UI2.WinForms.Guna2Button();
            LoginPageCreateAccountButton = new Guna.UI2.WinForms.Guna2Button();
            LoginPageForgotPasswordButton = new Guna.UI2.WinForms.Guna2Button();
            guna2ImageButton1 = new Guna.UI2.WinForms.Guna2ImageButton();
            ((System.ComponentModel.ISupportInitialize)LoginPageVLCPLayer).BeginInit();
            SuspendLayout();
            // 
            // loginPageErrorTextbox
            // 
            loginPageErrorTextbox.Location = new Point(809, 425);
            loginPageErrorTextbox.Name = "loginPageErrorTextbox";
            loginPageErrorTextbox.Size = new Size(226, 23);
            loginPageErrorTextbox.TabIndex = 5;
            loginPageErrorTextbox.Visible = false;
            // 
            // label1
            // 
            label1.ForeColor = SystemColors.ButtonFace;
            label1.Location = new Point(85, 294);
            label1.Name = "label1";
            label1.Size = new Size(100, 23);
            label1.TabIndex = 10;
            label1.Text = "Remember me";
            // 
            // LoginPageVLCPLayer
            // 
            LoginPageVLCPLayer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LoginPageVLCPLayer.BackColor = Color.Black;
            LoginPageVLCPLayer.Location = new Point(490, 46);
            LoginPageVLCPLayer.MediaPlayer = null;
            LoginPageVLCPLayer.Name = "LoginPageVLCPLayer";
            LoginPageVLCPLayer.Size = new Size(798, 638);
            LoginPageVLCPLayer.TabIndex = 13;
            // 
            // loginPageUsernameTextbox
            // 
            loginPageUsernameTextbox.AutoRoundedCorners = true;
            loginPageUsernameTextbox.BorderColor = Color.FromArgb(234, 153, 149);
            loginPageUsernameTextbox.BorderRadius = 17;
            loginPageUsernameTextbox.CustomizableEdges = customizableEdges1;
            loginPageUsernameTextbox.DefaultText = "";
            loginPageUsernameTextbox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            loginPageUsernameTextbox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            loginPageUsernameTextbox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            loginPageUsernameTextbox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            loginPageUsernameTextbox.FillColor = Color.FromArgb(35, 34, 50);
            loginPageUsernameTextbox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            loginPageUsernameTextbox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            loginPageUsernameTextbox.ForeColor = Color.FromArgb(35, 34, 50);
            loginPageUsernameTextbox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            loginPageUsernameTextbox.Location = new Point(31, 162);
            loginPageUsernameTextbox.Name = "loginPageUsernameTextbox";
            loginPageUsernameTextbox.PasswordChar = '\0';
            loginPageUsernameTextbox.PlaceholderForeColor = Color.Azure;
            loginPageUsernameTextbox.PlaceholderText = "Username";
            loginPageUsernameTextbox.SelectedText = "";
            loginPageUsernameTextbox.ShadowDecoration.CustomizableEdges = customizableEdges2;
            loginPageUsernameTextbox.Size = new Size(200, 36);
            loginPageUsernameTextbox.TabIndex = 15;
            // 
            // LoginPagePasswordTextbox
            // 
            LoginPagePasswordTextbox.AutoRoundedCorners = true;
            LoginPagePasswordTextbox.BorderColor = Color.FromArgb(234, 153, 149);
            LoginPagePasswordTextbox.BorderRadius = 17;
            LoginPagePasswordTextbox.CustomizableEdges = customizableEdges3;
            LoginPagePasswordTextbox.DefaultText = "";
            LoginPagePasswordTextbox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            LoginPagePasswordTextbox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            LoginPagePasswordTextbox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            LoginPagePasswordTextbox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            LoginPagePasswordTextbox.FillColor = Color.FromArgb(35, 34, 50);
            LoginPagePasswordTextbox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            LoginPagePasswordTextbox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LoginPagePasswordTextbox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            LoginPagePasswordTextbox.Location = new Point(31, 236);
            LoginPagePasswordTextbox.Name = "LoginPagePasswordTextbox";
            LoginPagePasswordTextbox.PasswordChar = '\0';
            LoginPagePasswordTextbox.PlaceholderForeColor = Color.Azure;
            LoginPagePasswordTextbox.PlaceholderText = "Password";
            LoginPagePasswordTextbox.SelectedText = "";
            LoginPagePasswordTextbox.ShadowDecoration.CustomizableEdges = customizableEdges4;
            LoginPagePasswordTextbox.Size = new Size(200, 36);
            LoginPagePasswordTextbox.TabIndex = 16;
            // 
            // LoginPageRememberMeToggle
            // 
            LoginPageRememberMeToggle.CheckedState.BorderColor = Color.Indigo;
            LoginPageRememberMeToggle.CheckedState.FillColor = Color.Maroon;
            LoginPageRememberMeToggle.CheckedState.InnerBorderColor = Color.White;
            LoginPageRememberMeToggle.CheckedState.InnerColor = Color.White;
            LoginPageRememberMeToggle.CustomizableEdges = customizableEdges5;
            LoginPageRememberMeToggle.Location = new Point(44, 294);
            LoginPageRememberMeToggle.Name = "LoginPageRememberMeToggle";
            LoginPageRememberMeToggle.ShadowDecoration.CustomizableEdges = customizableEdges6;
            LoginPageRememberMeToggle.Size = new Size(35, 20);
            LoginPageRememberMeToggle.TabIndex = 17;
            LoginPageRememberMeToggle.UncheckedState.BorderColor = Color.FromArgb(234, 153, 149);
            LoginPageRememberMeToggle.UncheckedState.BorderThickness = 2;
            LoginPageRememberMeToggle.UncheckedState.FillColor = Color.FromArgb(35, 34, 50);
            LoginPageRememberMeToggle.UncheckedState.InnerBorderColor = Color.White;
            LoginPageRememberMeToggle.UncheckedState.InnerColor = Color.FromArgb(234, 153, 149);
            // 
            // loginPageLoginButton
            // 
            loginPageLoginButton.Animated = true;
            loginPageLoginButton.AutoRoundedCorners = true;
            loginPageLoginButton.BorderRadius = 18;
            loginPageLoginButton.BorderThickness = 2;
            loginPageLoginButton.CustomizableEdges = customizableEdges7;
            loginPageLoginButton.DisabledState.BorderColor = Color.DarkGray;
            loginPageLoginButton.DisabledState.CustomBorderColor = Color.DarkGray;
            loginPageLoginButton.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            loginPageLoginButton.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            loginPageLoginButton.FillColor = Color.FromArgb(193, 20, 137);
            loginPageLoginButton.Font = new Font("Segoe UI", 9F);
            loginPageLoginButton.ForeColor = Color.White;
            loginPageLoginButton.Location = new Point(31, 347);
            loginPageLoginButton.Name = "loginPageLoginButton";
            loginPageLoginButton.ShadowDecoration.CustomizableEdges = customizableEdges8;
            loginPageLoginButton.Size = new Size(120, 38);
            loginPageLoginButton.TabIndex = 18;
            loginPageLoginButton.Text = "Login";
            // 
            // LoginPageCreateAccountButton
            // 
            LoginPageCreateAccountButton.Animated = true;
            LoginPageCreateAccountButton.AutoRoundedCorners = true;
            LoginPageCreateAccountButton.BorderRadius = 18;
            LoginPageCreateAccountButton.BorderThickness = 2;
            LoginPageCreateAccountButton.CustomizableEdges = customizableEdges9;
            LoginPageCreateAccountButton.DisabledState.BorderColor = Color.DarkGray;
            LoginPageCreateAccountButton.DisabledState.CustomBorderColor = Color.DarkGray;
            LoginPageCreateAccountButton.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            LoginPageCreateAccountButton.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            LoginPageCreateAccountButton.FillColor = Color.FromArgb(193, 20, 137);
            LoginPageCreateAccountButton.Font = new Font("Segoe UI", 9F);
            LoginPageCreateAccountButton.ForeColor = Color.White;
            LoginPageCreateAccountButton.Location = new Point(182, 347);
            LoginPageCreateAccountButton.Name = "LoginPageCreateAccountButton";
            LoginPageCreateAccountButton.ShadowDecoration.CustomizableEdges = customizableEdges10;
            LoginPageCreateAccountButton.Size = new Size(120, 38);
            LoginPageCreateAccountButton.TabIndex = 19;
            LoginPageCreateAccountButton.Text = "Create account";
            LoginPageCreateAccountButton.Click += LoginPageCreateAccountButton_Click_1;
            // 
            // LoginPageForgotPasswordButton
            // 
            LoginPageForgotPasswordButton.Animated = true;
            LoginPageForgotPasswordButton.AutoRoundedCorners = true;
            LoginPageForgotPasswordButton.BorderRadius = 10;
            LoginPageForgotPasswordButton.BorderThickness = 2;
            LoginPageForgotPasswordButton.CustomizableEdges = customizableEdges11;
            LoginPageForgotPasswordButton.DisabledState.BorderColor = Color.DarkGray;
            LoginPageForgotPasswordButton.DisabledState.CustomBorderColor = Color.DarkGray;
            LoginPageForgotPasswordButton.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            LoginPageForgotPasswordButton.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            LoginPageForgotPasswordButton.FillColor = Color.FromArgb(193, 20, 137);
            LoginPageForgotPasswordButton.Font = new Font("Segoe UI", 9F);
            LoginPageForgotPasswordButton.ForeColor = Color.White;
            LoginPageForgotPasswordButton.Location = new Point(199, 294);
            LoginPageForgotPasswordButton.Name = "LoginPageForgotPasswordButton";
            LoginPageForgotPasswordButton.ShadowDecoration.CustomizableEdges = customizableEdges12;
            LoginPageForgotPasswordButton.Size = new Size(134, 23);
            LoginPageForgotPasswordButton.TabIndex = 20;
            LoginPageForgotPasswordButton.Text = "Forgot Password?";
            // 
            // guna2ImageButton1
            // 
            guna2ImageButton1.CheckedState.ImageSize = new Size(64, 64);
            guna2ImageButton1.HoverState.ImageSize = new Size(64, 64);
            guna2ImageButton1.Image = (Image)resources.GetObject("guna2ImageButton1.Image");
            guna2ImageButton1.ImageOffset = new Point(0, 0);
            guna2ImageButton1.ImageRotate = 0F;
            guna2ImageButton1.Location = new Point(1140, 2);
            guna2ImageButton1.Name = "guna2ImageButton1";
            guna2ImageButton1.PressedState.ImageSize = new Size(64, 64);
            guna2ImageButton1.ShadowDecoration.CustomizableEdges = customizableEdges13;
            guna2ImageButton1.Size = new Size(41, 20);
            guna2ImageButton1.TabIndex = 21;
            guna2ImageButton1.Click += loginPageExitButton_Click;
            // 
            // LoginPage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(35, 34, 50);
            ClientSize = new Size(1284, 681);
            Controls.Add(guna2ImageButton1);
            Controls.Add(LoginPageForgotPasswordButton);
            Controls.Add(LoginPageCreateAccountButton);
            Controls.Add(loginPageLoginButton);
            Controls.Add(LoginPageRememberMeToggle);
            Controls.Add(LoginPagePasswordTextbox);
            Controls.Add(loginPageUsernameTextbox);
            Controls.Add(LoginPageVLCPLayer);
            Controls.Add(label1);
            Controls.Add(loginPageErrorTextbox);
            Name = "LoginPage";
            WindowState = FormWindowState.Minimized;
            ((System.ComponentModel.ISupportInitialize)LoginPageVLCPLayer).EndInit();
            ResumeLayout(false);
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

#endregion
        private Label loginPageErrorTextbox;
        private Label label1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private LibVLCSharp.WinForms.VideoView LoginPageVLCPLayer;
        private Guna.UI2.WinForms.Guna2TextBox loginPageUsernameTextbox;
        private Guna.UI2.WinForms.Guna2TextBox LoginPagePasswordTextbox;
        private Guna.UI2.WinForms.Guna2ToggleSwitch LoginPageRememberMeToggle;
        private Guna.UI2.WinForms.Guna2Button loginPageLoginButton;
        private Guna.UI2.WinForms.Guna2Button LoginPageCreateAccountButton;
        private Guna.UI2.WinForms.Guna2Button LoginPageForgotPasswordButton;
        private Guna.UI2.WinForms.Guna2ImageButton guna2ImageButton1;
    }
}
