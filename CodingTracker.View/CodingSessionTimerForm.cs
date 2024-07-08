using System;
using CodingTracker.Common.IApplicationLoggers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodingTracker.Common.ICodingSessions;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.View.FormSwitchers;
using CodingTracker.View.FormControllers;
using CodingTracker.Common.CodingGoalDTOManagers;
using CodingTracker.Common.CodingGoalDTOs;
using CodingTracker.Common.ISessionGoalCountDownTimers;
using CodingTracker.View.FormFactories;
namespace CodingTracker.View
{
    public partial class CodingSessionTimerForm : Form
    {
        private readonly IApplicationLogger _appLogger;
        private readonly ISessionLogic _codingSesison;
        private readonly IErrorHandler _errorHandler;
        private readonly IFormSwitcher _formSwitcher;
        private readonly IFormController _formController;
        private readonly ICodingGoalDTOManager _codingGoalDTOManager;
        private readonly CodingGoalDTO _currentGoalDTO;
        private readonly ISessionGoalCountDownTimer _sessionCountDownTimer;
        private readonly IFormFactory _formFactory;
        public event Action<TimeSpan> TimeChanged;
        private TimeSpan totalTime;
        public event Action CountDownFinished;



        public CodingSessionTimerForm(IApplicationLogger appLogger, ISessionLogic codingSession, ICodingGoalDTOManager codingGoalDTOManager, ISessionGoalCountDownTimer countdownTimer, IFormSwitcher formSwitcher, IFormController formController, IFormFactory formFactory)
        {
            _appLogger = appLogger;
            _codingSesison = codingSession;
            _codingGoalDTOManager = codingGoalDTOManager;
            _sessionCountDownTimer = countdownTimer;
            _formSwitcher = formSwitcher;
            _formController = formController;
            InitializeComponent();
            _currentGoalDTO = _codingGoalDTOManager.GetCurrentCodingGoalDTO();
            this.Load += CodingSessionTimerForm_Load;
            _sessionCountDownTimer = countdownTimer;
            _sessionCountDownTimer.TimeChanged += UpdateTimeRemainingDisplay;
            _sessionCountDownTimer.CountDownFinished += HandleCountDownFinished;
            _formFactory = formFactory;
        }

        private void CodingSessionTimerForm_Load(object sender, EventArgs e)
        {
            using (var activity = new Activity(nameof(CodingSessionTimerForm_Load)).Start())
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug($"Loading Coding Session Timer Form. TraceID: {activity.TraceId}");

                try
                {
                    InitializeProgressBar();
                    SetCodingGoalDisplay();


                    if (_currentGoalDTO != null)
                    {

                        _sessionCountDownTimer.InitializeAndStartTimer(_currentGoalDTO.GoalHours, _currentGoalDTO.GoalMinutes);
                    }

                    stopwatch.Stop();
                    _appLogger.Info($"Coding Session Timer Form loaded successfully. Execution Time: {stopwatch.ElapsedMilliseconds}ms, TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Error loading Coding Session Timer Form. Execution Time: {stopwatch.ElapsedMilliseconds}ms, Error: {ex.Message}, TraceID: {activity.TraceId}", ex);
                }
            }
        }

        public string FormatCodingGoalTime()
        {
            using (var activity = new Activity(nameof(FormatCodingGoalTime)).Start())
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug($"Formatting coding goal time. TraceID: {activity.TraceId}");

                if (_currentGoalDTO == null)
                {
                    _appLogger.Warning($"_currentGoalDTO is null. TraceID: {activity.TraceId}");
                    stopwatch.Stop();
                    return "00:00";
                }

                string formattedTime = $"{_currentGoalDTO.GoalHours:D2}:{_currentGoalDTO.GoalMinutes:D2}";
                stopwatch.Stop();
                _appLogger.Info($"Coding goal time formatted to {formattedTime}. Execution Time: {stopwatch.ElapsedMilliseconds}ms, TraceID: {activity.TraceId}");
                return formattedTime;
            }
        }

        private void SetCodingGoalDisplay()
        {
            using (var activity = new Activity(nameof(SetCodingGoalDisplay)).Start())
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug($"Updating Coding Session Timer Label. TraceID: {activity.TraceId}");

                string formattedTime = FormatCodingGoalTime();
                CodingSessionTimerPageTimerLabel.Text = formattedTime;

                stopwatch.Stop();
                _appLogger.Info($"Coding Session Timer Label updated to {formattedTime}. Execution Time: {stopwatch.ElapsedMilliseconds}ms, TraceID: {activity.TraceId}");
            }
        }


        private void UpdateTimeRemainingDisplay(TimeSpan timeRemaining)
        {
            using (var activity = new Activity(nameof(UpdateTimeRemainingDisplay)).Start())
            {
                try
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        CodingSessionTimerPageTimerLabel.Text = timeRemaining.ToString(@"hh\:mm\:ss");
                        double percentage = CalculateRemainingPercentage(timeRemaining);
                        UpdateProgressBar(percentage);
                        stopwatch.Stop();

                    }));
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred while updating time remaining display. RemainingTime: {timeRemaining}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }

        private void HandleCountDownFinished()
        {
            Invoke((MethodInvoker)(() =>
            {
                CodingSessionTimerPageTimerLabel.Text = "00:00:00";
                MessageBox.Show("Countdown complete!");
            }));
        }

        private void CodingTimerPageEndSessionButton_Click(object sender, EventArgs e)
        {
            _sessionCountDownTimer.StopCountDownTimer();
            _codingSesison.EndSession();
            _formFactory.CreateMainPage();
            _formSwitcher.SwitchToMainPage();
  
        }

        private void CodingSesisonTimerPageNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void CodingSessionTimerForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                CodingSesisonTimerPageNotifyIcon.Visible = true;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();

                CodingSesisonTimerPageNotifyIcon.Visible = true;
                CodingSesisonTimerPageNotifyIcon.ShowBalloonTip(3000, "Application Minimized", "The application is still running in the background.", ToolTipIcon.Info);
            }
            else
            {
                CodingSesisonTimerPageNotifyIcon.Dispose();
            }
        }

        private void InitializeProgressBar()
        {
            if (_currentGoalDTO != null)
            {
                totalTime = new TimeSpan(_currentGoalDTO.GoalHours, _currentGoalDTO.GoalMinutes, 0);
                CodingSessionTimerPageProgressBar.Maximum = 100;
            }
        }

        private double CalculateRemainingPercentage(TimeSpan timeRemaining)
        {
            double totalSeconds = totalTime.TotalSeconds;
            double remainingSeconds = timeRemaining.TotalSeconds;
            return (remainingSeconds / totalSeconds) * 100;
        }

        private void UpdateProgressBar(double percentage)
        {
            CodingSessionTimerPageProgressBar.Value = (int)percentage;
        }

        private void MainPageExitControlMinimizeButton_Click(object sender, EventArgs e)
        {

        }
    }
}
