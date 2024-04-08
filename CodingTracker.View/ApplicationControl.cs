using CodingTracker.Common.IApplicationControls;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.ICodingSessions;
using CodingTracker.Common.IDatabaseManagers;
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
        private readonly ICodingSession _codingSession;
        private readonly IDatabaseManager _databaseManager;
        public bool ApplicationIsRunning { get; private set; }



        public ApplicationControl(IApplicationLogger appLogger, ICodingSession codingSession, IDatabaseManager databaseManager)
        {
            ApplicationIsRunning = false; // Set to false instead of true to ensure that processes don't run or exit prematurely or unintentionally.
            _appLogger = appLogger;
            _codingSession = codingSession;
            _databaseManager = databaseManager;
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

                    if (_codingSession.CheckIfCodingSessionActive())
                    {
                        _codingSession.EndSession();
                        _appLogger.Info($"Active coding session saved. TraceID: {activity.TraceId}");
                    }

                    _databaseManager.CloseDatabaseConnection();
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