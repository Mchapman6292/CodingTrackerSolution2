using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.CodingSessions;
using System.Diagnostics;
using CodingTracker.Business.CodingSessionService.UserIdServices;
using CodingTracker.View.FormService;
using CodingTracker.Common.BusinessInterfaces.ICodingSessionManagers;
using CodingTracker.Common.BusinessInterfaces.IAuthenticationServices;
using CodingTracker.Common.BusinessInterfaces;


namespace CodingTracker.View
{
    public partial class CodingSessionPage : Form
    {
        private readonly IFormController _formController;
        private readonly IFormSwitcher _formSwitcher;
        private readonly ISessionGoalCountDownTimer _goalCountDownTimer;
        private readonly IInputValidator _inputValidator;
        private readonly IErrorHandler _errorHandler;
        private readonly IApplicationLogger _appLogger;
        private readonly ICodingSessionManager _codingSessionManager;
        private readonly IAuthenticationService _authenticationService;

        private readonly UserIdService _userIdService;


        private int _goalHours;
        private int _goalMinutes;
        public CodingSessionPage(IFormSwitcher formSwitcher, IFormController formController,ISessionGoalCountDownTimer goalCountDownTimer, IInputValidator inputValidator, IApplicationLogger appLogger, ICodingSessionManager codingSessionManager, IAuthenticationService authenticationService, UserIdService idService)
        {
            InitializeComponent();
            _formSwitcher = formSwitcher;
            _formController = formController;
            _goalCountDownTimer = goalCountDownTimer;
            _inputValidator = inputValidator;
            _appLogger = appLogger;
            _codingSessionManager = codingSessionManager;
            _authenticationService = authenticationService;
            _userIdService = idService;
        }

        private async void CodingSessionPageStartSessionButton_Click(object sender, EventArgs e)
        {
            await _appLogger.LogActivityAsync(nameof(CodingSessionPageStartSessionButton_Click),
                async activity =>
                {
                    _appLogger.Info($"Starting {nameof(CodingSessionPageStartSessionButton_Click)} TraceId: {activity.TraceId}.");
                },
                async activity =>
                {
                    try
                    {
                        this.Hide();
                        _appLogger.Info($"Form hidden. TraceId: {activity.TraceId}");

                        _formSwitcher.SwitchToCodingSessionTimer();
                        _appLogger.Info($"Switched to Coding Session Timer. TraceId: {activity.TraceId}");

                        int currentUserId =  _userIdService.GetCurrentUserId();
                        _appLogger.Info($"Retrieved current user ID: {currentUserId}. TraceId: {activity.TraceId}");

                        _codingSessionManager.StartCodingSession(activity, currentUserId);
                        _appLogger.Info($"Coding session started for user ID: {currentUserId}. TraceId: {activity.TraceId}");
                    }
                    catch (Exception ex)
                    {
                        _appLogger.Error($"Error starting coding session. TraceId: {activity.TraceId}", ex);
                    }
                });
        }


        private async void CodingSesionPageEndSessionButton_Click(object sender, EventArgs e)
        {
            await _appLogger.LogActivityAsync(nameof(CodingSesionPageEndSessionButton_Click),
                async activity =>
                {
                    _appLogger.Info($"Starting {nameof(CodingSesionPageEndSessionButton_Click)} TraceId: {activity.TraceId}.");
                },
                async activity =>
                {
                    try
                    {
                        _codingSessionManager.EndCodingSession(activity);
                        _appLogger.Info($"Coding session ended. TraceId: {activity.TraceId}");

                        this.Hide();
                        _appLogger.Info($"Form hidden. TraceId: {activity.TraceId}");

                        _formSwitcher.SwitchToMainPage();
                        _appLogger.Info($"Switched to main page. TraceId: {activity.TraceId}");
                    }
                    catch (Exception ex)
                    {
                        _appLogger.Error($"Error ending coding session. TraceId: {activity.TraceId}", ex);
                    }
                });
        }



        private void CodingSessionPageConfirmSessionGoalButton_Click(object sender, EventArgs e)
        {
            var activity = new Activity(nameof(CodingSessionPageConfirmSessionGoalButton_Click)).Start();
            var stopwatch = Stopwatch.StartNew();

            int goalHours = Convert.ToInt32(CodingGoalSetHourToggle.Value);
            int goalMinutes = Convert.ToInt32(CodingGoalSetMinToggle.Value);



            activity.Stop();
        }


        public bool ValidateGoalTime(int hours, int minutes)
        {
            var activity = new Activity(nameof(ValidateGoalTime)).Start();
            var stopwatch = Stopwatch.StartNew();

            _appLogger.Debug($"Validating goal time. Hours: {hours}, Minutes: {minutes}, TraceID: {activity.TraceId}");

            int totalMinutes = hours * 60 + minutes;

            if (totalMinutes < 15 || totalMinutes > 24 * 60)
            {
                _appLogger.Warning($"Invalid goal time. Total time {totalMinutes} minutes is outside the valid range. TraceID: {activity.TraceId}");
                stopwatch.Stop();
                activity.Stop();
                return false;
            }

            _appLogger.Info($"Goal time validated successfully. Total time: {totalMinutes} minutes. Execution Time: {stopwatch.ElapsedMilliseconds}ms, TraceID: {activity.TraceId}");

            stopwatch.Stop();
            activity.Stop();
            return true;
        }

        private void CodingSessionPageCodingGoalToggle_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = CodingSessionPageCodingGoalToggle.Checked;

            CodingGoalSetHourToggle.Enabled = isChecked;
            CodingGoalSetMinToggle.Enabled = isChecked;
        }

        public void MinimizeToTray()
        {
            this.Hide();
            this.CodingSessionPageNotifyIcon.Visible = true;

            this.CodingSessionPageNotifyIcon.MouseDoubleClick += (sender, e) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.CodingSessionPageNotifyIcon.Visible = false;
            };
        }


        private void CodingSessionPageHomeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            _formSwitcher.SwitchToMainPage();
        }
    }
}
