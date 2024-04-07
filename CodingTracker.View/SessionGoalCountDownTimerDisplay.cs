using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.ISessionGoalCountDownTimers;
using System.Windows.Forms;
using System;
using System.Diagnostics;

namespace CodingTracker.View.SessionGoalCountDownTimers
{
    public class SessionGoalCountDownTimerDisplay : IDisposable, ISessionGoalCountDownTimer
    {
        private readonly IApplicationLogger _appLogger;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private TimeSpan _maxTime;
        private System.Windows.Forms.Timer _timer;
        public event Action<TimeSpan> TimeChanged;
        public event Action CountDownFinished;

        public bool IsRunning => _stopwatch.IsRunning;

        public SessionGoalCountDownTimerDisplay(IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
            _timer = new System.Windows.Forms.Timer();
            _timer.Tick += Timer_Tick; // Event for timer tick
        }

        public void Timer_Tick(object sender, EventArgs e)
        {
            if (_stopwatch.Elapsed < _maxTime)
            {
                TimeChanged?.Invoke(_maxTime - _stopwatch.Elapsed);
            }
            else
            {
                _timer.Stop();
                _stopwatch.Stop();
                CountDownFinished?.Invoke();
                _appLogger.Info($"Timer countdown finished.");
            }
        }


        public void SetCountDownTimer(int minutes, int seconds = 0)
        {
            var methodStopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(SetCountDownTimer)).Start())
            {
                _maxTime = TimeSpan.FromSeconds(minutes * 60 + seconds);
                methodStopwatch.Stop();
                _appLogger.Debug($"Timer set to {_maxTime}. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
            }
        }

        public void StartCountDownTimer()
        {
            var methodStopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(StartCountDownTimer)).Start())
            {
                _appLogger.Debug($"Starting timer. TraceID: {activity.TraceId}");

                if (_maxTime > TimeSpan.Zero)
                {
                    _stopwatch.Start();
                    CheckTimeCountDownTimer();
                    methodStopwatch.Stop();
                    _appLogger.Debug($"Timer started. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                else
                {
                    methodStopwatch.Stop();
                    _appLogger.Warning($"Timer start requested but maxTime is zero. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
            }
        }

        public void InitializeAndStartTimer(int minutes, int seconds = 0)
        {
            SetCountDownTimer(minutes, seconds);
            StartCountDownTimer();
        }

        public void CheckTimeCountDownTimer()
        {
            var methodStopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(CheckTimeCountDownTimer)).Start())
            {
                if (_stopwatch.Elapsed < _maxTime)
                {
                    TimeChanged?.Invoke(_maxTime - _stopwatch.Elapsed);
                }
                else
                {
                    _stopwatch.Stop();
                    CountDownFinished?.Invoke();
                    methodStopwatch.Stop();
                    _appLogger.Info($"Timer countdown finished. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
            }
        }

        public void StopCountDownTimer()
        {
            var methodStopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(StopCountDownTimer)).Start())
            {
                _stopwatch.Stop();
                TimeChanged?.Invoke(_maxTime - _stopwatch.Elapsed);
                methodStopwatch.Stop();
                _appLogger.Debug($"Timer stopped. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
            }
        }

        public void Reset()
        {
            var methodStopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(Reset)).Start())
            {
                _stopwatch.Reset();
                TimeChanged?.Invoke(_maxTime);
                methodStopwatch.Stop();
                _appLogger.Debug($"Timer reset. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
            }
        }

        public void Dispose()
        {
            var methodStopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(Dispose)).Start())
            {
                _stopwatch?.Stop();
                methodStopwatch.Stop();
                _appLogger.Debug($"Timer disposed. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
            }
        }
    }
}
