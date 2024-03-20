using CodingTracker.Common.ISessionGoalCountDownTimer;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.IApplicationLoggers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodingTracker.View
{
    public partial class CodingSessionPage : Form
    {
        private readonly ISessionGoalCountDownTimer _countDownTimer;
        private readonly IInputValidator _inputValidator;
        private readonly IApplicationLogger _appLogger;
        public CodingSessionPage(ISessionGoalCountDownTimer countDownTimer, IInputValidator inputValidator, IApplicationLogger logger)
        {
            InitializeComponent();
            _countDownTimer = countDownTimer;
            _inputValidator = inputValidator;
            _countDownTimer.TimeChanged += UpdateTimeRemainingDisplay;
            _countDownTimer.CountDownFinished += HandleCountDownFinished;
            CodingSessionPageSessionGoalTextBox.KeyDown += CodingSessionPageSessionGoalTextBox_KeyDown;
            _appLogger = logger;
        }

        private void CodingSessionPageStartSessionButton_Click(object sender, EventArgs e)
        {

        }

        private void CodingSessionPageMinimiseButton_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }

        private void CodingSessionPageSetSessionGoalButton_Click(object sender, EventArgs e)
        {
            CodingSessionPageSessionGoalInputPanel.Visible = true;
        }



        private void CodingSessionPageSessionGoalTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true; // to prevent the ding sound on pressing enter
                ProcessSessionGoalInput();
            }
        }


        private void ProcessSessionGoalInput()
        {
            string input = CodingSessionPageSessionGoalTextBox.Text;

            // First, check if the input is in the correct HH:mm format
            if (!_inputValidator.IsValidTimeFormatHHMM(input))
            {
                MessageBox.Show("Invalid time format. Please enter time in HH:mm format.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Then, try to parse the time
            if (_inputValidator.TryParseTime(input, out TimeSpan sessionGoal))
            {
                int minutes = (int)sessionGoal.TotalMinutes;
                int seconds = sessionGoal.Seconds;
                CodingSessionPageSessionGoalInputPanel.Visible = false; // Hide the panel
            }
            else
            {
                // This block may be redundant if TryParseTime only fails due to format issues
                // But it's kept here for completeness and future-proofing
                MessageBox.Show("Invalid time format. Please ensure the time is correct.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateTimeRemainingDisplay(TimeSpan timeRemaining)
        {
            // Update UI with the remaining time
        }

        private void HandleCountDownFinished()
        {
            // Update UI when countdown finishes
        }

    }
}
