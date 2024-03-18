using CodingTracker.Common.IApplicationLoggers;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;

namespace CodingTracker.Business.Codingtimers
{
    public class CodingTimer
    {
        private readonly IApplicationLogger _appLogger;
        private TimeSpan _remainingTime;
        private System.Timers.Timer timer;
        private bool _isFirstTick;

        public CodingTimer(IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
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

            public void Tick()
            {
            if (_remainingTime > TimeSpan.Zero)
                {
                _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));
            }
        }

        public TimeSpan GetRemainingTime()
        {
            return _remainingTime;
                }

        public bool IsFinished()
                {
            return _remainingTime <= TimeSpan.Zero;
            }
        }

    }
}
