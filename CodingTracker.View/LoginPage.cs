using CodingTracker.Common.IApplicationControls;
using CodingTracker.Common.ILoginManagers;
using CodingTracker.Common.IApplicationLoggers;
using System;
using System.IO;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System.Diagnostics;

namespace CodingTracker.View
{
    public partial class LoginPage : Form
    {
        private readonly ILoginManager _loginManager;
        private readonly IApplicationControl _appControl;
        private readonly IApplicationLogger _appLogger;
        private LibVLC _libVLC;
        private VideoView _videoView;

        public LoginPage(ILoginManager loginManager, IApplicationControl appControl, IApplicationLogger applogger)
        {
            _loginManager = loginManager;
            _appControl = appControl;
            _appLogger = applogger;
            InitializeComponent();
            InitializeVLCPlayer();
        }

        private void InitializeVLCPlayer()
        {
            Core.Initialize();
            _libVLC = new LibVLC();

            _videoView = new VideoView
            {
                Location = new Point(838, 188),
                Size = new Size(316, 234),
                MediaPlayer = new MediaPlayer(_libVLC)
            };
            this.Controls.Add(_videoView);
            _videoView.BringToFront();

            string videoFilePath = CodingTracker.View.Properties.Settings.Default.VLCPath;
            if (File.Exists(videoFilePath))
            {
                var media = new Media(_libVLC, new Uri(videoFilePath));
                media.AddOption("input-repeat=65535"); // Loop the video indefinitely
                _videoView.MediaPlayer.Play(media);

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
            string password = loginPagePasswordTextbox.Text;

            var loginCredentials = _loginManager.ValidateLogin(username, password);

            if (loginCredentials != null)
            {
                MessageBox.Show("Login successful.");
            }
            else
            {
                loginPageErrorTextbox.Visible = true;
                loginPageErrorTextbox.Text = "Login failed. Please check your username and password.";
            }
        }

        private void loginPageExitButton_Click(object sender, EventArgs e)
        {
            _appControl.ExitApplication();
        }
    }
}