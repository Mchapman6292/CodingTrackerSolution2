using CodingTracker.Common.IApplicationControls;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.DataInterfaces.ICodingTrackerDbContexts;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.DataInterfaces.ICodingSessionRepositories;
using CodingTracker.Common.ICodingSessionManagers;

namespace CodingTracker.Business.ApplicationControls
{
    public class ApplicationControl : IApplicationControl
    {
        private readonly IApplicationLogger _appLogger;
        private readonly ICodingTrackerDbContext _context;
        private readonly ICodingSessionManager _codingSessionManager;
        public bool ApplicationIsRunning { get; private set; }



        public ApplicationControl(IApplicationLogger appLogger, ICodingTrackerDbContext entityContext, ICodingSessionManager codingSessionManager)
        {
            ApplicationIsRunning = false; // Set to false instead of true to ensure that processes don't run or exit prematurely or unintentionally.
            _appLogger = appLogger;
            _context = entityContext;
            _codingSessionManager = codingSessionManager;
        }

        public void StartApplication()
        {
            ApplicationIsRunning = true;
        }

        public async Task ExitApplication()
        {
            using var activity = new Activity(nameof(ExitApplication)).Start();
            _appLogger.Info($"Starting {nameof(ExitApplication)}. TraceID: {activity.TraceId}");

            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (_codingSessionManager.CheckIfCodingSessionActive())
                {
                    _codingSessionManager.EndCodingSession(activity);
                    _appLogger.Info($"Active coding session saved. TraceID: {activity.TraceId}");
                }

                await _context.SaveChangesAsync();

                stopwatch.Stop();
                _appLogger.Info($"{nameof(ExitApplication)} completed. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");

                Application.Exit();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _appLogger.Error($"An error occurred during {nameof(ExitApplication)}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
            }
        }
    }
}