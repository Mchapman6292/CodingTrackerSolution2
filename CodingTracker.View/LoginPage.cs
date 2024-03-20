using CodingTracker.Common.IApplicationControls;
using CodingTracker.Common.ILoginManagers;
using CodingTracker.Common.IApplicationLoggers;
using System;
using System.IO;
using System.Windows.Forms;
using AxWMPLib;
using System.Diagnostics;

namespace CodingTracker.View
{
    public partial class LoginPage : Form
    {
        private readonly ILoginManager _loginManager;
        private readonly IApplicationControl _appControl;
        private readonly IApplicationLogger _appLogger;

        public LoginPage(ILoginManager loginManager, IApplicationControl appControl, IApplicationLogger applogger)
        {
            _loginManager = loginManager;
            _appControl = appControl;
            _appLogger = applogger;
            InitializeComponent();
            InitializeMediaPlayer();

            LoginPageMonitorImage.uiMode = "none";
            LoginPageMonitorImage.stretchToFit = true;
            LoginPageMonitorImage.settings.setMode("loop", true);

            string videoFilePath = Path.Combine(Application.StartupPath, "FormMedia", "PcScreenPixelArt.mp4");

            if (File.Exists(videoFilePath))
            {
                LoginPageMonitorImage.URL = videoFilePath;
            }
            else
            {
                MessageBox.Show("Video file not found at the path: " + videoFilePath);
            }

            // Attach the error event handler correctly
            LoginPageMonitorImage.ErrorEvent += LoginPageMonitorImage_ErrorEvent;
        }



        private void LoginPage_Load(object sender, EventArgs e)
        {
            string videoFilePath = Path.Combine(Application.StartupPath, "FormMedia", "PcScreenPixelArt.mp4");
            _appLogger.Debug($"Loading video from path: {videoFilePath}");

            if (File.Exists(videoFilePath))
            {
                LoginPageMonitorImage.URL = videoFilePath;
                LoginPageMonitorImage.Ctlcontrols.play();
                _appLogger.Info($"Video loaded successfully from path: {videoFilePath}");
            }
            else
            {
                _appLogger.Warning($"Video file not found at path: {videoFilePath}");
                MessageBox.Show("Video file not found at the path: " + videoFilePath);
            }
        }

        private void InitializeMediaPlayer()
        {
            const string methodName = nameof(InitializeMediaPlayer);
            using (var activity = new Activity(methodName).Start())
            {
                _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}");

                try
                {
                    LoginPageMonitorImage.uiMode = "none";
                    LoginPageMonitorImage.stretchToFit = true;

                    // Autoplay & Loop settings 
                    LoginPageMonitorImage.settings.autoStart = true;
                    LoginPageMonitorImage.settings.setMode("loop", true);

                    string videoFilePath = Path.Combine(Application.StartupPath, "FormMedia", "PcScreenPixelArt.mp4");
                    _appLogger.Debug($"{methodName}: Video file path is {videoFilePath}. TraceID: {activity.TraceId}");

                    if (File.Exists(videoFilePath))
                    {
                        LoginPageMonitorImage.URL = videoFilePath;
                        _appLogger.Info($"{methodName}: Video file found and URL set. TraceID: {activity.TraceId}");
                    }
                    else
                    {
                        _appLogger.Warning($"{methodName}: Video file not found at {videoFilePath}. TraceID: {activity.TraceId}");
                        MessageBox.Show("Video file not found at the path: " + videoFilePath);
                    }

                    LoginPageMonitorImage.PlayStateChange += LoginPageMonitorImage_PlayStateChange;
                    LoginPageMonitorImage.ErrorEvent += LoginPageMonitorImage_ErrorEvent;
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"{methodName}: Exception occurred - {ex.Message}. TraceID: {activity.TraceId}", ex);
                    MessageBox.Show("An error occurred while initializing the video player.");
                }
            }
        }

        private void LoginPageMonitorImage_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            _appLogger.Debug($"MediaPlayer PlayStateChange: {e.newState}");
            // Handle different states as needed
        }


        private void LoginPageMonitorImage_ErrorEvent(object sender, EventArgs e)
        {
            MessageBox.Show("An error occurred during video playback.");
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

        private void LoginPageMonitorImage_Enter(object sender, EventArgs e)
        {

        }

        private void LoginPageMonitorImage_Enter_1(object sender, EventArgs e)
        {

        }
    }
}
