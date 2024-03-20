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

namespace CodingTracker.View
{
    public partial class CodingSessionTimer : Form
    {
        private readonly IApplicationLogger _appLogger;
        public CodingSessionTimer(IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
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
            var methodStopwatch = Stopwatch.StartNew();
            try
            {
                _appLogger.Debug("Handling countdown finished.");

                // Handle the completion of the countdown, e.g.,
                // Show a message, disable/enable controls, etc.

                methodStopwatch.Stop();
                _appLogger.Info($"Handled countdown finished. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                methodStopwatch.Stop();
                _appLogger.Error($"An error occurred while handling countdown finished. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. Error: {ex.Message}", ex);
            }
        }
    }
}
