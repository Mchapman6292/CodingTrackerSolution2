using CodingTracker.Common.IApplicationLoggers;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Business.Codingtimers
{
    public class CodingTimerssss
    {
        private readonly IApplicationLogger _appLogger;
        private TimeSpan _elapsedTime;

        public CodingTimerssss(IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
        }


        public void Timer_Tick(object sender, EventArgs e)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            using (var activity = new Activity(nameof(Timer_Tick)).Start())
            {
                try
                {
                    _appLogger.Debug($"Starting {nameof(Timer_Tick)}. TraceID: {activity.TraceId}");

                    _elapsedTime = _elapsedTime.Add(TimeSpan.FromSeconds(1));

                    CodingSessionTimerPageTimerLabel.Text = _elapsedTime.ToString(@"hh\:mm\:ss");

                    stopwatch.Stop();
                    _appLogger.Debug($"{nameof(Timer_Tick)} completed in {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"{nameof(Timer_Tick)} failed after {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    throw;
                }
            }
        }
    }
}
