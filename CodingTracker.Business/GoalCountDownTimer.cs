using System;
using System.Diagnostics;

namespace CodingTracker.Business
{
    public class BusinessCountDownTimer : IDisposable
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private TimeSpan _maxTime;
        public event Action<TimeSpan> TimeChanged;
        public event Action CountDownFinished;

        public bool IsRunning => _stopwatch.IsRunning;

        public BusinessCountDownTimer()
        {
        }

        public void SetTime(int minutes, int seconds = 0)
        {
            _maxTime = TimeSpan.FromSeconds(minutes * 60 + seconds);
        }

        public void Start()
        {
            if (_maxTime > TimeSpan.Zero)
            {
                _stopwatch.Start();
                CheckTime();
            }
        }

        private void CheckTime()
        {
            if (_stopwatch.Elapsed < _maxTime)
            {
                TimeChanged?.Invoke(_maxTime - _stopwatch.Elapsed);
            }
            else
            {
                _stopwatch.Stop();
                CountDownFinished?.Invoke();
            }
        }

        public void Stop()
        {
            _stopwatch.Stop();
            TimeChanged?.Invoke(_maxTime - _stopwatch.Elapsed);
        }

        public void Reset()
        {
            _stopwatch.Reset();
            TimeChanged?.Invoke(_maxTime);
        }

        public void Dispose()
        {
            _stopwatch?.Stop();
        }
    }
}