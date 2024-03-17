using CodingTracker.Common.IApplicationLoggers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodingTracker.View
{
    public partial class CodingSessionCountDownTimer : Form
    {
        private TimeSpan _remainingTime;
        private System.Windows.Forms.Timer _uiTimer;
        private readonly IApplicationLogger _appLogger;
        private Stopwatch _stopwatch;

        public CodingSessionCountDownTimer(IApplicationLogger appLogger, TimeSpan sessionLength)
        {
            InitializeComponent();

            _appLogger = appLogger;
            _remainingTime = sessionLength;
            CodingSessionTimerPageTimerLabel.Text = _remainingTime.ToString(@"hh\:mm\:ss");

            _uiTimer = new System.Windows.Forms.Timer();
            _uiTimer.Interval = 1000;
            _uiTimer.Tick += UiTimer_Tick;

            StartTimer();
        }

        private void StartTimer()
        {
            using (var activity = new Activity(nameof(StartTimer)).Start())
            {
                _appLogger.Debug($"Starting timer. TraceID: {activity.TraceId}");

                _stopwatch = Stopwatch.StartNew();
                _uiTimer.Start();
            }
        }

        private void UiTimer_Tick(object sender, EventArgs e)
        {
            if (_remainingTime.TotalSeconds > 0)
            {
                _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));
                CodingSessionTimerPageTimerLabel.Text = _remainingTime.ToString(@"hh\:mm\:ss");
            }
            else
            {
                _uiTimer.Stop();
                _stopwatch.Stop();
                CodingSessionTimerPageTimerLabel.Text = "Session complete!!";

                using (var activity = new Activity(nameof(UiTimer_Tick)).Start())
                {
                    _appLogger.Debug($"Timer completed. Total Duration: {_stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
            }
        }

        private void CodingSessionTimerPageTimerLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
