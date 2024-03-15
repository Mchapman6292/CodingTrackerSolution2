using CodingTracker.Common.IApplicationControls;
using CodingTracker.Common.IApplicationLoggers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Business.ApplicationControls
{
    public class ApplicationControl : IApplicationControl
    {
        private readonly IApplicationLogger _appLogger;
        public bool ApplicationIsRunning { get; private set; }



        public ApplicationControl(IApplicationLogger appLogger)
        {
            ApplicationIsRunning = false; // Set to false instead of true to ensure that processes don't run or exit prematurely or unintentionally.
            _appLogger = appLogger;
        }

        public void StartApplication()
        {
            ApplicationIsRunning = true;
        }

        public void ExitApplication()
        {
            using (var activity = new Activity(nameof(ExitApplication)).Start())
            {
                _appLogger.Info($"Starting {nameof(ExitApplication)}. TraceID: {activity.TraceId}");

                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    if (CheckIfSessionActive())
                    {
                        SaveCurrentCodingSession();
                        _appLogger.Info($"Active coding session saved. TraceID: {activity.TraceId}");
                    }

                    CloseDatabaseConnections();
                    _appLogger.Info($"Database connections closed. TraceID: {activity.TraceId}");

                    stopwatch.Stop();
                    _appLogger.Info($"{nameof(ExitApplication)} completed. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");

                    Application.Exit();
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(ExitApplication)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }
    }
}