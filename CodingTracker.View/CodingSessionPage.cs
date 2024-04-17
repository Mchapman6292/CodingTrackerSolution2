using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodingTracker.View.FormControllers;
using CodingTracker.View.FormSwitchers;
using CodingTracker.Common.ICodingSessions;
using CodingTracker.Common.ISessionGoalCountDownTimers;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.CodingGoalDTOs;
using CodingTracker.Common.CodingGoalDTOManagers;
using CodingTracker.Common.IApplicationLoggers;
using System.Diagnostics;
using CodingTracker.Common.CodingSessionDTOs;


namespace CodingTracker.View
{
    public partial class CodingSessionPage : Form
    {
        private readonly IFormController _formController;
        private readonly IFormSwitcher _formSwitcher;
        private readonly ICodingSession _codingSession;
        private readonly ISessionGoalCountDownTimer _goalCountDownTimer;
        private readonly IInputValidator _inputValidator;
        private readonly IErrorHandler _errorHandler;
        private readonly ICodingGoalDTOManager _goalDTOManager;
        private readonly IApplicationLogger _appLogger;


        private int _goalHours;
        private int _goalMinutes;
        public CodingSessionPage(IFormSwitcher formSwitcher, IFormController formController, ICodingSession codingSession, ISessionGoalCountDownTimer goalCountDownTimer, IInputValidator inputValidator, ICodingGoalDTOManager goalDTOManager, IApplicationLogger appLogger)
        {
            InitializeComponent();
            _formSwitcher = formSwitcher;
            _formController = formController;
            _codingSession = codingSession;
            _goalCountDownTimer = goalCountDownTimer;
            _inputValidator = inputValidator;
            _goalDTOManager = goalDTOManager;
            _appLogger = appLogger;
        }

        private void CodingSessionPageStartSessionButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            _formSwitcher.SwitchToCodingSessionTimer();
            _codingSession.StartSession();

        }





        private void CodingSessionPageConfirmSessionGoalButton_Click(object sender, EventArgs e)
        {
            var activity = new Activity(nameof(CodingSessionPageConfirmSessionGoalButton_Click)).Start();
            var stopwatch = Stopwatch.StartNew();

            int goalHours = Convert.ToInt32(CodingGoalSetHourToggle.Value);
            int goalMinutes = Convert.ToInt32(CodingGoalSetMinToggle.Value);

            _appLogger.Debug($"Starting to confirm session goal with hours: {_goalHours}, minutes: {_goalMinutes}. TraceID: {activity.TraceId}");

            var codingGoalDTO = _goalDTOManager.CreateCodingGoalDTO(goalHours, goalMinutes);
            _goalCountDownTimer.setMaxTime();

            stopwatch.Stop();
            _appLogger.Info($"Session goal confirmed and CodingGoalDTO created. Hours: {codingGoalDTO.GoalHours}, Minutes: {codingGoalDTO.GoalMinutes}. Execution Time: {stopwatch.ElapsedMilliseconds}ms, TraceID: {activity.TraceId}");

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

        private void CodingSesionPageEndSessionButton_Click(object sender, EventArgs e)
        {
            _codingSession.EndSession();
            this.Hide();
            _formSwitcher.SwitchToMainPage();
        }

        private void CodingSessionPageHomeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            _formSwitcher.SwitchToMainPage();
        }
    }
}
