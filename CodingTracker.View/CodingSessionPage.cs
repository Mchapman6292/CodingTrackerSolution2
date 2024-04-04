using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodingTracker.View.IFormControllers;
using CodingTracker.View.IFormSwitchers;
using CodingTracker.Common.ICodingSessions;
using CodingTracker.Common.ISessionGoalCountDownTimers;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.IErrorHandlers;

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
        public CodingSessionPage(IFormSwitcher formSwitcher, IFormController formController, ICodingSession codingSession, ISessionGoalCountDownTimer goalCountDownTimer, IInputValidator inputValidator)
        {
            InitializeComponent();
            _formSwitcher = formSwitcher;
            _formController = formController;
            _codingSession = codingSession;
            _goalCountDownTimer = goalCountDownTimer;
            _inputValidator = inputValidator;
        }

        private void CodingSessionPageStartSessionButton_Click(object sender, EventArgs e)
        {
            _codingSession.StartSession();

        }

        private void CheckForCodingGoalInput(object sender, EventArgs e)
        {
            _errorHandler.CatchErrorsAndLogWithStopwatch(() =>
            {
                TextBox CodingSessionPageSessionGoalTextBox = sender as TextBox;

                if (_inputValidator.IsValidTimeFormatHHMM(CodingSessionPageSessionGoalTextBox.Text))
                {
                    _inputValidator.ParseHHMMStringInputToInt(CodingSessionPageSessionGoalTextBox.Text);
                }
            }, nameof(CheckForCodingGoalInput), false);
        }
    }
}
