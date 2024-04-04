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
using CodingTracker.View.IFormSwitchers;
using CodingTracker.View.IFormControllers;

namespace CodingTracker.View
{
    public partial class CodingSessionTimer : Form
    {
        private readonly IApplicationLogger _appLogger;
        private readonly ICodingSession _codingSesison;
        private readonly IErrorHandler _errorHandler;
        private readonly IFormSwitcher _formSwitcher;
        private readonly IFormController _formController;
        public CodingSessionTimer(IApplicationLogger appLogger, ICodingSession codingSession)
        {
            _appLogger = appLogger;
            _codingSesison = codingSession;
            InitializeComponent();
        }

        private void CodingSessionTimerPageTimerLabel_Click(object sender, EventArgs e)
        {

        }
        private void UpdateTimeRemainingDisplay(TimeSpan timeRemaining)
        {
            var methodStopwatch = Stopwatch.StartNew();
            try
            {
                _appLogger.Debug($"Updating time remaining display to {timeRemaining}.");

                // Update the UI element showing the time remaining, e.g.,
                // this.TimeRemainingLabel.Text = timeRemaining.ToString(@"hh\:mm\:ss");

                methodStopwatch.Stop();
                _appLogger.Info($"Updated time remaining display. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                methodStopwatch.Stop();
                _appLogger.Error($"An error occurred while updating time remaining display. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. Error: {ex.Message}", ex);
            }
        }

        private void HandleCountDownFinished()
        {
            throw new NotImplementedException();
        }

        private void CodingTimerPageEndSessionButton_Click(object sender, EventArgs e)
        {
            _codingSesison.EndSession();
            _formController.CloseCurrentForm();
            _formSwitcher.SwitchToMainPage();

        }
    }
}
