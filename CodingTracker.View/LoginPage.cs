using CodingTracker.Common.IApplicationControls;
using CodingTracker.Common.ILoginManagers;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.View.IFormControllers;
using CodingTracker.View.IFormSwitchers;
using CodingTracker.Common.ILoginManagers;
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
        private readonly ILoginManager _loginManager;
        private readonly IApplicationControl _appControl;
        private readonly IApplicationLogger _appLogger;
        private readonly ICredentialManager _credentialManager;
        private readonly IFormController _formController;
        private readonly IFormSwitcher _formSwitcher;
        private LibVLC _libVLC;
        private VideoView _videoView;

        public LoginPage(ILoginManager loginManager, IApplicationControl appControl, IApplicationLogger applogger, ICredentialManager credentialManager, IFormController formController, IFormSwitcher formSwitcher)
        {
            _loginManager = loginManager;
            _appControl = appControl;
            _appLogger = applogger;
            _credentialManager = credentialManager;
            _formController = formController;
            this.FormBorderStyle = FormBorderStyle.None;
            InitializeComponent();
            InitializeVLCPlayer();
            _formSwitcher = formSwitcher;
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

            string videoFilePath = CodingTracker.View.Properties.Settings.Default.VLCPath;
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


        private void loginPageLoginButton_Click(object sender, EventArgs e)
        {
            string username = loginPageUsernameTextbox.Text;
            string password = LoginPagePasswordTextbox.Text;

            var loginCredentials = _loginManager.ValidateLogin(username, password);

            if (loginCredentials != null)
            {
                _formSwitcher.SwitchToMainPage();
                _appLogger.Info("User logged in success.");
            }
            else
            {
                LoginPageDisplaySuccessMessage("Login failed. Please check your username and password.");
            }
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
    }
}