using CodingTracker.Common.IApplicationLoggers;
using System.Windows.Forms;
using System;
using System.Diagnostics;
using CodingTracker.Business.CodingSessionService;
using CodingTracker.Common.BusinessInterfaces;


namespace CodingTracker.View.SessionGoalCountDownTimers
{
    public class SessionGoalCountdownTimer : IDisposable, ISessionGoalCountDownTimer
    {
        private readonly IApplicationLogger _appLogger;
        private readonly ICodingSessionCountDownTimer _sessionCountDownTimer;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private TimeSpan _maxTime;
        private System.Windows.Forms.Timer _timer;
        public event Action<TimeSpan> TimeChanged;
        public event Action CountDownFinished;

        public bool IsRunning => _stopwatch.IsRunning;

        public SessionGoalCountdownTimer(IApplicationLogger appLogger, ICodingSessionCountDownTimer sessionCountDownTimer)
        {
            _appLogger = appLogger;
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1000;
            _timer.Tick += Timer_Tick; // Event for timer tick
            _sessionCountDownTimer = sessionCountDownTimer;
        }

        public void SetCountDownTimer(int minutes, int seconds = 0)
        {
            var methodStopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(SetCountDownTimer)).Start())
            {

                methodStopwatch.Stop();
                _appLogger.Debug($"Timer set to {_maxTime}. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
            }
        }

        public void StartCountDownTimer()
        {
            var methodStopwatch = Stopwatch.StartNew();
            TimeSpan maxTime = setMaxTime();
            using (var activity = new Activity(nameof(StartCountDownTimer)).Start())
            {
                _appLogger.Debug($"Starting timer. TraceID: {activity.TraceId}");

                if (_maxTime > TimeSpan.Zero)
                {
                

                    _stopwatch.Start();
                    _timer.Start();
                    CheckTimeCountDownTimer();
                    methodStopwatch.Stop();
                    _appLogger.Debug($"Timer started. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                else
                {
                    methodStopwatch.Stop();
                    _appLogger.Error($"Timer start requested but maxTime is zero. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
            }
        }

        public void InitializeAndStartTimer(int minutes, int seconds = 0)
        {
            SetCountDownTimer(minutes, seconds);
            StartCountDownTimer();
        }


        public void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_stopwatch.Elapsed < _maxTime)
                {
                    TimeChanged?.Invoke(_maxTime - _stopwatch.Elapsed);
                }
                else
                {
                    _timer.Stop();
                    _stopwatch.Stop(); // runs independently of the ui thread and is not affected by freezes in ui etc. 
                    CountDownFinished?.Invoke();
                    _appLogger.Info("Timer countdown finished.");
                }
            }
            catch (Exception ex)
            {
                _appLogger.Error($"An error occurred during Timer_Tick: {ex.Message}", ex);
            }
        }

        private TimeSpan CalculateRemainingTime()
        {
            return _maxTime - _stopwatch.Elapsed;
        }


        public TimeSpan setMaxTime()
        {
            throw new NotImplementedException();
        }


        public TimeSpan ConvertGoalHoursAndMinsToTimeSpan(int goalHours, int goalMins)
        {
            TimeSpan goalTimeSpan = new TimeSpan(goalHours, goalMins, 0);
            return goalTimeSpan;
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
