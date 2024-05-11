﻿using System.Diagnostics;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.IApplicationLoggers;
using System.Linq.Expressions;
using CodingTracker.Common.ICodingSessions;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.ICodingSessionTimers;
using CodingTracker.Common.IDatabaseSessionReads;
using CodingTracker.Common.CodingGoalDTOManagers;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.CodingSessionDTOManagers;
using CodingTracker.Common.IDatabaseSessionInserts;
using CodingTracker.Common.ICredentialManagers;
using CodingTracker.Data.DatabaseSessionInserts;
using CodingTracker.Business.SessionCalculators;
using CodingTracker.Common.INewDatabaseReads;
using CodingTracker.Common.IQueryBuilders;


// method to record start & end time
// logic to hold recorded times & view them
// user should be able to input start & end times manually j     

namespace CodingTracker.Business.CodingSessions
{
    public class CodingSession : ICodingSession
    {
        private readonly IInputValidator _inputValidator;
        private readonly IApplicationLogger _appLogger;
        private readonly IErrorHandler _errorHandler;
        private readonly ICodingSessionTimer _sessionTimer;
        private readonly ICodingSessionDTOManager _sessionDTOManager;
        private readonly IDatabaseSessionRead _databaseSessionRead;
        private readonly ICodingGoalDTOManager _goalDTOManager;
        private readonly IDatabaseSessionInsert _databaseSessionInsert;
        private readonly ICredentialManager _credentialManager;
        private readonly ISessionCalculator _sessionCalculator;
        private readonly IQueryBuilder _queryBuilder;
        private readonly INewDatabaseRead _newDatabaseRead;
        private readonly int _userId;
        private readonly int _sessionId;
        public bool IsStopWatchEnabled = false;
        private bool isCodingSessionActive = false;


        public CodingSession(IInputValidator validator, IApplicationLogger appLogger, IErrorHandler errorHandler, ICodingSessionTimer sessionTimer, ICodingSessionDTOManager sessionDTOManager, IDatabaseSessionRead databaseSessionRead, ICodingGoalDTOManager goalDTOManager, IDatabaseSessionInsert databaseSessionInsert, ICredentialManager credentialManager, ISessionCalculator sessionCalculator, IQueryBuilder queryBuilder, INewDatabaseRead newDatabaseRead)
        {
            _inputValidator = validator;
            _appLogger = appLogger;
            _errorHandler = errorHandler;
            _sessionTimer = sessionTimer;
            _sessionDTOManager = sessionDTOManager;
            _databaseSessionRead = databaseSessionRead;
            _credentialManager = credentialManager;
            _goalDTOManager = goalDTOManager;
            _databaseSessionInsert = databaseSessionInsert;
            _sessionCalculator = sessionCalculator;
            _userId = _databaseSessionRead.GetSessionIdWithMostRecentLogin();
            _sessionId = _databaseSessionRead.GetSessionIdWithMostRecentLogin();
            _queryBuilder = queryBuilder;
            _newDatabaseRead = newDatabaseRead;
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
                _sessionDTOManager.SetSessionStartDate();
                _sessionDTOManager.SetSessionStartTime();
                _sessionTimer.StartCodingSessionTimer();

                stopwatch.Stop();
                _appLogger.Info($"New session started successfully for userId: {sessionDto.userId}. Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");
            }
        }

        public void InsertCodingSessionDTOToDatabase()
        {
            _appLogger.LogActivity(nameof(InsertCodingSessionDTOToDatabase), activity =>
            {

                ActivityTraceId traceId = activity.TraceId;
                _appLogger.Info($"Starting {nameof(InsertCodingSessionDTOToDatabase)}. TraceID: {traceId}.");
            }, activity =>
            {
                CodingSessionDTO currentSessionDTO = _sessionDTOManager.GetCurrentSessionDTO();
                string command = "INSERT";
                _newDatabaseRead.HandleCodingSessionsTableOperations
                (
                    columnsToSelect: new List<string> { "StartDate", "StartTime", "EndDate", "EndTime", "DurationSeconds", "DurationHHMM", "GoalHHMM", "GoalReached" },
                    sqlCommand: command,
                    startDate: currentSessionDTO.startDate,
                    startTime: currentSessionDTO.startTime,
                    endDate: currentSessionDTO.endDate,
                    endTime: currentSessionDTO.endTime,
                    durationSeconds: currentSessionDTO.durationSeconds,
                    durationHHMM: currentSessionDTO.durationHHMM,
                    goalHHMM: currentSessionDTO.goalHHMM,
                    goalReached: currentSessionDTO.goalReached
                );
            });
        }
        

        public void EndSession()
        {
            using (var activity = new Activity(nameof(EndSession)).Start())
            {
              
               CodingSessionDTO currentSessionDTO = _sessionDTOManager.GetCurrentSessionDTO();

                var stopwatch = Stopwatch.StartNew();

                _appLogger.Debug($" {nameof(EndSession)} initiated. TraceID: {activity.TraceId}");

                isCodingSessionActive = false;

                _sessionTimer.EndCodingSessionTimer();
                _sessionDTOManager.SetSessionEndDate();
                _sessionDTOManager.SetSessionEndTime();


                double durationSeconds = _sessionCalculator.CalculateDurationSeconds();
                TimeSpan durationTimeSpan = _sessionDTOManager.ConvertDurationSecondsToTimeSpan(durationSeconds);
                string goalHHMM = _goalDTOManager.FormatCodingGoalHoursMinsToString();
                string durationHHMM = _sessionDTOManager.ConvertDurationSecondsIntoStringHHMM(durationSeconds);


                _sessionDTOManager.UpdateCurrentSessionDTO(_sessionId, _userId, currentSessionDTO.startDate,currentSessionDTO.startTime, currentSessionDTO.endDate,currentSessionDTO.endTime, durationSeconds, durationHHMM, goalHHMM);

                _appLogger.Info($"EndSession UpdateCurrentSessionDTO parameters: sessionId:{_sessionId}, UserId{_userId}, StartDate{currentSessionDTO.startDate}, StartTime{currentSessionDTO.startTime}, EndDate{currentSessionDTO.endDate}, EndTime{currentSessionDTO.endTime}, DurationSeconds{durationSeconds}, DurationHHMM{durationHHMM}, GoalHHMM{goalHHMM}");
                InsertCodingSessionDTOToDatabase();

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

                for (int i = 1; i <= 29; i++)
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