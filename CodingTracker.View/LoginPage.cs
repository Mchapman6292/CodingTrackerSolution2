using CodingTracker.Common.IApplicationControls;
using CodingTracker.Common.IAuthtenticationServices;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.View.FormControllers;
using CodingTracker.View.FormSwitchers;

using System;
using System.IO;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System.Diagnostics;
using CodingTracker.Common.ICredentialManagers;
using CodingTracker.Data.CredentialManagers;
using System.Drawing.Drawing2D;

namespace CodingTracker.View
{
    public partial class LoginPage : Form
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IApplicationControl _appControl;
        private readonly IApplicationLogger _appLogger;
        private readonly ICredentialManager _credentialManager;
        private readonly IFormController _formController;
        private readonly IFormSwitcher _formSwitcher;
        private LibVLC _libVLC;
        private VideoView _videoView;

        public LoginPage(IAuthenticationService authenticationService, IApplicationControl appControl, IApplicationLogger applogger, ICredentialManager credentialManager, IFormController formController, IFormSwitcher formSwitcher)
        {
            _authenticationService = authenticationService;
            _appControl = appControl;
            _appLogger = applogger;
            _credentialManager = credentialManager;
            _formController = formController;
            _formSwitcher = formSwitcher;
            this.FormBorderStyle = FormBorderStyle.None;
            InitializeComponent();
            InitializeVLCPlayer();
            loginPageUsernameTextbox.Enter += LoginPagePasswordTextbox_Enter;
            loginPageUsernameTextbox.Leave += LoginPageUsernameTextbox_Leave;
            LoginPagePasswordTextbox.Enter += LoginPagePasswordTextbox_Enter;
            LoginPagePasswordTextbox.Leave += LoginPagePasswordTextbox_Leave;
            LoginPageRememberMeToggle.Checked = Properties.Settings.Default.RememberMe;
            LoadSavedCredentials();
        }



        // Custom paint method: Draws the rounded corners for the form.
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            GraphicsPath path = new GraphicsPath();
            int radius = 40;

            // Top left arc
            path.AddArc(0, 0, radius, radius, 180, 90);
            // Top right arc
            path.AddArc(Width - radius, 0, radius, radius, 270, 90);
            // Bottom right arc
            path.AddArc(Width - radius, Height - radius, radius, radius, 0, 90);
            // Bottom left arc
            path.AddArc(0, Height - radius, radius, radius, 90, 90);

            path.CloseFigure();

            this.Region = new Region(path);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawPath(new Pen(Color.Black, 2), path);
        }

        private void InitializeVLCPlayer()
        {
            Core.Initialize();
            _libVLC = new LibVLC();

            _videoView = new VideoView
            {
                MediaPlayer = new MediaPlayer(_libVLC)
            };

            _videoView.Location = new Point(0, 0);
            _videoView.Size = new Size(888, 581); ; // This has been set manually through trial & error. If the size is set to a smaller figure black borders appear around the mp4 despite borders being disabled. 

            LoginPageMediaPanel.Controls.Add(_videoView);
            _videoView.BringToFront();

            string videoFilePath = Properties.Settings.Default.VLCPath;
            if (File.Exists(videoFilePath))
            {
                var media = new Media(_libVLC, new Uri(videoFilePath));
                media.AddOption("input-repeat=65535"); // Loop the video indefinitely
                _videoView.MediaPlayer.Play(media);

                _videoView.MediaPlayer.Scale = 0; // = 0 so that it fills to the size of the MediaPanel.


                _appLogger.Info($"VLC player loaded video from {videoFilePath}");
            }
            else
            {
                _appLogger.Warning($"VLC player video file not found at {videoFilePath}");
                MessageBox.Show("Video file not found at the specified path: " + videoFilePath);
            }
        }


        private void LoadSavedCredentials()
        {
            try
            {

                if (Properties.Settings.Default.RememberMe)
                {
                    var lastUsername = Properties.Settings.Default.LastUsername;
                    if (!string.IsNullOrEmpty(lastUsername))
                    {
                        loginPageUsernameTextbox.Text = lastUsername;
                        LoginPageRememberMeToggle.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error loading saved credentials: {ex.Message}");
            }
        }




        private async void loginPageLoginButton_Click(object sender, EventArgs e)
        {
            await _appLogger.LogActivityAsync(nameof(loginPageLoginButton_Click),
                async activity =>
                {
                    _appLogger.Info($"Login activity started. TraceId:{activity.TraceId}");
                },
                async activity =>  // Note the added 'activity' parameter here
                {
                    try
                    {
                        string username = loginPageUsernameTextbox.Text;
                        string password = LoginPagePasswordTextbox.Text;
                        bool isValidLogin = await _authenticationService.AuthenticateLogin(username, password, activity);

                        if (isValidLogin)
                        {
                            if (LoginPageRememberMeToggle.Checked)
                            {
                                Properties.Settings.Default.LastUsername = username;
                                Properties.Settings.Default.Save();
                            }
                            this.Hide();
                            _formSwitcher.SwitchToMainPage();
                            _appLogger.Info($"User logged in successfully. TraceId:{activity.TraceId}");
                        }
                        else
                        {
                            LoginPageDisplaySuccessMessage("Login failed. Please check your username and password.");
                            _appLogger.Warning($"Login failed for user '{username}'. TraceId:{activity.TraceId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _appLogger.Error($"Error during login process. TraceId:{activity.TraceId}", ex);
                    }
                }
            );
        }







        private void AccountCreatedSuccessfully(string message)
        {
            _appLogger.Debug("AccountCreatedSuccessfully method called.");

            this.Invoke((MethodInvoker)(() =>
            {
                _appLogger.Debug("Inside Invoke method of AccountCreatedSuccessfully.");
                LoginPageDisplaySuccessMessage(message);
            }));
        }

        private void LoginPageDisplaySuccessMessage(string message)
        {

            LoginPageCreationSuccessTextBox.Text = message;
        }



        private void LoginPageCreateAccountButton_Click_1(object sender, EventArgs e)
        {
            var createAccountPage = _formSwitcher.SwitchToCreateAccountPage();
            createAccountPage.AccountCreatedCallback = AccountCreatedSuccessfully;

        }

        private void LoginPageExitControlBox_Click(object sender, EventArgs e)
        {
            _appControl.ExitApplication();

        }

        private void loginPageLoginButton_Click_1(object sender, EventArgs e)
        {

        }

        private void LoginPageForgotPasswordButton_Click(object sender, EventArgs e)
        {
          
        }

        private void LoginPageRememberMeToggle_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RememberMe = LoginPageRememberMeToggle.Checked;
            Properties.Settings.Default.Save();
        }



        private void loginPageUsernameTextbox_TextChanged(object sender, EventArgs e)
        {

        }


        private void InitializePlaceholderText()
        {

            LoginPagePasswordTextbox.Text = "Password";
            LoginPagePasswordTextbox.ForeColor = Color.Gray;
            LoginPagePasswordTextbox.PasswordChar = '\0';
        }

        private void LoginPagePasswordTextbox_Enter(object sender, EventArgs e)
        {
            if (LoginPagePasswordTextbox.Text == "Password")
            {
                LoginPagePasswordTextbox.Text = "";
                LoginPagePasswordTextbox.ForeColor = Color.Black; // Or whatever your default text color is
                LoginPagePasswordTextbox.PasswordChar = '●'; // Set this to your desired password character
            }
        }



        private void LoginPageUsernameTextbox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(loginPageUsernameTextbox.Text))
            {
                loginPageUsernameTextbox.Text = "Username";
                loginPageUsernameTextbox.ForeColor = Color.White;
            }
        }

        private void LoginPagePasswordTextbox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(LoginPagePasswordTextbox.Text))
            {
                LoginPagePasswordTextbox.Text = "Password";
                LoginPagePasswordTextbox.ForeColor = Color.Gray;
                LoginPagePasswordTextbox.PasswordChar = '\0'; // Clear the PasswordChar property
            }
        }
    }
}


