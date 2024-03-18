using CodingTracker.Common.IApplicationLoggers;
using System;
using System.Diagnostics;

namespace CodingTracker.Business
{
    public class SessionGoalCountDownTimer : IDisposable
    {
        private readonly IApplicationLogger _appLogger;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private TimeSpan _maxTime;
        public event Action<TimeSpan> TimeChanged;
        public event Action CountDownFinished;

        public bool IsRunning => _stopwatch.IsRunning;

        public SessionGoalCountDownTimer(IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
        }

        public void SetTime(int minutes, int seconds = 0)
        {
            var methodStopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(SetTime)).Start())
            {
                _maxTime = TimeSpan.FromSeconds(minutes * 60 + seconds);
                methodStopwatch.Stop();
                _appLogger.Debug($"Timer set to {_maxTime}. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
            }
        }

        public void Start()
        {
            var methodStopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(Start)).Start())
            {
                _appLogger.Debug($"Starting timer. TraceID: {activity.TraceId}");

                if (_maxTime > TimeSpan.Zero)
                {
                    _stopwatch.Start();
                    CheckTime();
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

        private void CheckTime()
        {
            var methodStopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(CheckTime)).Start())
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

        public void Stop()
        {
            var methodStopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(Stop)).Start())
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
