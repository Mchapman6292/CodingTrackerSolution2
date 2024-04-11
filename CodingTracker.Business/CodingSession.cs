using System.Diagnostics;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.ICodingGoals;
using System.Linq.Expressions;
using CodingTracker.Common.ICodingSessions;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.ICodingSessionTimers;
using CodingTracker.Common.IDatabaseSessionReads;
using CodingTracker.Common.CodingGoalDTOManagers;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.CodingSessionDTOManagers;
using CodingTracker.Common.IDatabaseSessionInserts;
using CodingTracker.Data.DatabaseSessionInserts;


// method to record start & end time
// logic to hold recorded times & view them
// user should be able to input start & end times manually j     

namespace CodingTracker.Business.CodingSessions
{
    public class CodingSession : ICodingSession
    {
        private readonly IInputValidator _inputValidator;
        private readonly IApplicationLogger _appLogger;
        private readonly ICodingGoal _codingGoal;
        private readonly IErrorHandler _errorHandler;
        private readonly ICodingSessionTimer _sessionTimer;
        private readonly ICodingSessionDTOManager _sessionDTOManager;
        private readonly IDatabaseSessionRead _databaseSessionRead;
        private readonly ICodingGoalDTOManager _goalDTOManager;
        private readonly IDatabaseSessionInsert _databaseSessionInsert;
        private readonly int _userId;
        private readonly int _sessionId;
        public bool IsStopWatchEnabled = false;
        private bool isCodingSessionActive = false;


        public CodingSession(IInputValidator validator, IApplicationLogger appLogger, ICodingGoal codingGoal, IErrorHandler errorHandler, ICodingSessionTimer sessionTimer, ICodingSessionDTOManager sessionDTOManager, IDatabaseSessionRead databaseSessionRead, ICodingGoalDTOManager goalDTOManager, IDatabaseSessionInsert databaseSessionInsert)
        {
            _inputValidator = validator;
            _appLogger = appLogger;
            _codingGoal = codingGoal;
            _errorHandler = errorHandler;
            _sessionTimer = sessionTimer;
            _sessionDTOManager = sessionDTOManager;
            _databaseSessionRead = databaseSessionRead;
            _userId = _databaseSessionRead.GetUserIdWithMostRecentLogin();
            _sessionId = _databaseSessionRead.GetSessionIdWithMostRecentLogin();
            _goalDTOManager = goalDTOManager;
            _databaseSessionInsert = databaseSessionInsert;
        }


        public void StartSession()
        {
            using (var activity = new Activity(nameof(StartSession)).Start())
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug($"Attempting to start a new session. TraceID: {activity.TraceId}");

                bool sessionAlreadyActive = CheckIfCodingSessionActive();
                if (sessionAlreadyActive)
                {
                    _appLogger.Warning($"Cannot start a new session because another session is already active. TraceID: {activity.TraceId}");
                    stopwatch.Stop();
                    return;
                }

                isCodingSessionActive = true;
                var sessionDto = _sessionDTOManager.CreateAndReturnCurrentSessionDTO();
                _sessionDTOManager.SetSessionStartTimeAndDate();
                _sessionTimer.StartCodingSessionTimer();

                stopwatch.Stop();
                _appLogger.Info($"New session started successfully for UserId: {sessionDto.UserId}. Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");
            }
        }

        public void EndSession()
        {
            using (var activity = new Activity(nameof(EndSession)).Start())
            {
              
               CodingSessionDTO currentSessionDTO = _sessionDTOManager.GetCurrentSessionDTO();
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug($"Ending {nameof(EndSession)}. TraceID: {activity.TraceId}");

                isCodingSessionActive = false;

                _sessionTimer.EndCodingSessionTimer();
                _sessionDTOManager.SetSessionEndTimeAndDate();
                _sessionDTOManager.CalculateDurationMinutes();

                _sessionDTOManager.UpdateCurrentSessionDTO(_sessionId, _userId, currentSessionDTO.StartTime, currentSessionDTO.EndTime, currentSessionDTO.StartDate, currentSessionDTO.EndDate, currentSessionDTO.DurationMinutes);
                _databaseSessionInsert.InsertSession();

                stopwatch.Stop();
                _appLogger.Info($"Session ended, IsStopWatchEnabled: {IsStopWatchEnabled}, isCodingSessionActive: {isCodingSessionActive}, Trace ID: {activity.TraceId}, Execution Time: {stopwatch.ElapsedMilliseconds}ms");
            }
        }



        public bool CheckIfCodingSessionActive()
        {
            using (var activity = new Activity(nameof(CheckIfCodingSessionActive)).Start())
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug($"Checking if session is active. TraceID: {activity.TraceId}");

                bool isActive = isCodingSessionActive;

                stopwatch.Stop();
                _appLogger.Info($"Coding session active status: {isActive}, Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");

                return isActive;
            }
        }



        public List<DateTime> GetDatesPrevious28days() // Potential mismatch with sql lite db dates?
        {
            using (var activity = new Activity(nameof(GetDatesPrevious28days)).Start())
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug($"Getting dates for the previous 28 days. TraceID: {activity.TraceId}");

                List<DateTime> dates = new List<DateTime>();
                DateTime today = DateTime.Today;

                for (int i = 1; i <= 28; i++)
                {
                    dates.Add(today.AddDays(-i));
                }

                stopwatch.Stop();
                _appLogger.Info($"Retrieved dates for the previous 28 days. Count: {dates.Count}, Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");

                return dates;
            }
        }


    }
}