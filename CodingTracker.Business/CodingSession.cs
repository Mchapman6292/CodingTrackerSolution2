using System.Diagnostics;
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
using CodingTracker.Business.AuthenticationServices;
using CodingTracker.Common.IAuthenticationServices;
using System.Text.RegularExpressions;
using CodingTracker.Common.CurrentCodingSessions;


// method to record start & end time
// logic to hold recorded times & view them
// user should be able to input start & end times manually j     

namespace CodingTracker.Business.CodingSessions
{
    public class CodingSession : ICodingSession
    {
        private readonly CurrentCodingsession _currentCodingSession;
        public int CurrentSessionId { get; set; } = 0; // Default value indicating not set.
        public int CurrentUserId { get; set; } = 0;
        public DateOnly CurrentStartDate { get; set; } = DateOnly.MinValue;
        public DateTime CurrentStartTime { get; set; } = DateTime.MinValue;
        public DateOnly CurrentEndDate { get; set; } = DateOnly.MaxValue; 
        public DateTime CurrentEndTime { get; set; } = DateTime.MinValue;
        public double CurrentDurationSeconds { get; set; } = -1;
        public string CurrentDurationHHMM { get; set; } = "00:00";
        public string CurrentGoalHHMM { get; set; } = "00:00";
        public int CurrentGoalReached { get; set; } = 0;
    
        public bool IsStopWatchEnabled = false;

        private bool isCodingSessionActive = false;

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
        private readonly IAuthenticationService _authenticationService;

  

        public CodingSession(IInputValidator validator, IApplicationLogger appLogger, IErrorHandler errorHandler, ICodingSessionTimer sessionTimer, ICodingSessionDTOManager sessionDTOManager, IDatabaseSessionRead databaseSessionRead, ICodingGoalDTOManager goalDTOManager, IDatabaseSessionInsert databaseSessionInsert, ICredentialManager credentialManager, ISessionCalculator sessionCalculator, IQueryBuilder queryBuilder, INewDatabaseRead newDatabaseRead, IAuthenticationService authenticationService,CurrentCodingsession currentCodingSession)
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
            _queryBuilder = queryBuilder;
            _newDatabaseRead = newDatabaseRead;
            _authenticationService = authenticationService;
            _currentCodingSession = currentCodingSession;
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
                
                
                _sessionDTOManager.SetSessionStartTime();
                _sessionTimer.StartCodingSessionTimer();

                stopwatch.Stop();
                _appLogger.Info($"New session started successfully for userId: {_currentCodingSession.UserId}. Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");
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
                _newDatabaseRead.InsertIntoCodingSessionTable(currentSessionDTO.userId, currentSessionDTO.startDate, currentSessionDTO.startTime, currentSessionDTO.endDate, currentSessionDTO.endTime, currentSessionDTO.durationSeconds, currentSessionDTO.durationHHMM, currentSessionDTO.goalHHMM, currentSessionDTO.goalReached);
 ;
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


                _sessionDTOManager.UpdateCurrentSessionDTO(currentSessionDTO.userId, currentSessionDTO.startDate,currentSessionDTO.startTime, currentSessionDTO.endDate,currentSessionDTO.endTime, durationSeconds, durationHHMM, goalHHMM);

                _appLogger.Info($"EndSession UpdateCurrentSessionDTO parameters: UserId{currentSessionDTO.userId}, StartDate{currentSessionDTO.startDate}, StartTime{currentSessionDTO.startTime}, EndDate{currentSessionDTO.endDate}, EndTime{currentSessionDTO.endTime}, DurationSeconds{durationSeconds}, DurationHHMM{durationHHMM}, GoalHHMM{goalHHMM}");
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

        public void UpdateUserId(int userId)
        {
            _appLogger.LogActivity(nameof(UpdateUserId), activity =>
            {
                _appLogger.Info($"Starting {nameof(UpdateUserId)} with initial UserId = {_currentCodingSession.UserId}. TraceID: {activity.TraceId}");
            },
            activity =>
            {
                if (userId <= 0)
                {
                    _appLogger.Error($"UserId parameter of {nameof(UpdateUserId)} is invalid ({userId}). Must be greater than zero.");
                }
                else
                {
                    _currentCodingSession.UserId = userId;
                    _appLogger.Info($"UserId set to {_currentCodingSession.UserId}. TraceID: {activity.TraceId}");
                }
            });
        }


        public void SetOrUpdateSessionId(int sessionId)
        {
            _appLogger.LogActivity(nameof(SetOrUpdateSessionId), activity =>
            {
                _appLogger.Info($"Starting {nameof(SetOrUpdateSessionId)} with initial SessionId = {_currentCodingSession.SessionId}. TraceID: {activity.TraceId}");
            },
            activity =>
            {
                if (sessionId <= 0)
                {
                    _appLogger.Error($"SessionId parameter of {nameof(SetOrUpdateSessionId)} is invalid ({sessionId}). Must be greater than zero.");
                }
                else
                {
                    _currentCodingSession.SessionId = sessionId;
                    _appLogger.Info($"SessionId set to {_currentCodingSession.SessionId}. TraceID: {activity.TraceId}");
                }
            });
        }

        public void SetOrUpdateStartDate(DateOnly startDate)
        {
            _appLogger.LogActivity(nameof(SetOrUpdateStartDate), activity =>
            {
                _appLogger.Info($"Starting {nameof(SetOrUpdateStartDate)} with initial StartDate = {_currentCodingSession.StartDate}. TraceID: {activity.TraceId}");
            },
            activity =>
            {
                if (startDate == DateOnly.MinValue)
                {
                    _appLogger.Info($"StartDate parameter of {nameof(SetOrUpdateStartDate)} is not set. It is DateTime.MinValue.");
                }
                else
                {
                    _currentCodingSession.StartDate = startDate;
                    _appLogger.Info($"StartDate set to {_currentCodingSession.StartDate}. TraceID: {activity.TraceId}");
                }
            });
        }

        public void SetOrUpdateStartTime(DateTime startTime)
        {
            _appLogger.LogActivity(nameof(SetOrUpdateStartTime), activity =>
            {
                _appLogger.Info($"Starting {nameof(SetOrUpdateStartTime)} Parameter value = {startTime}. TraceID:{activity.TraceId}.");
            },
            activity =>
            {
                if (startTime == DateTime.MinValue)
                {
                    _appLogger.Info($"StartTime parameter of {nameof(SetOrUpdateStartTime)} is not set. ItIs DateTime.MinValue.");
                }
                else
                {
                    _currentCodingSession.StartTime = startTime;
                    _appLogger.Info($"StartTime set to {_currentCodingSession.StartTime}. TraceID: {activity.TraceId}.");
                }
            });
        }

        public void SetOrUpdateEndDate(DateOnly endDate)
        {
            _appLogger.LogActivity(nameof(SetOrUpdateEndDate), activity =>
            {
                _appLogger.Info($"Starting {nameof(SetOrUpdateEndDate)} with initial EndDate = {_currentCodingSession.EndDate}. TraceID: {activity.TraceId}");
            },
            activity =>
            {
                if (endDate == DateOnly.MaxValue)
                {
                    _appLogger.Info($"EndDate parameter of {nameof(SetOrUpdateEndDate)} is set to MaxValue, indicating an unset or unrealistic date.");
                }
                else
                {
                    _currentCodingSession.EndDate = endDate;
                    _appLogger.Info($"EndDate set to {_currentCodingSession.EndDate}. TraceID: {activity.TraceId}");
                }
            });
        }

        public void SetOrUpdateEndTime(DateTime endTime)
        {
            _appLogger.LogActivity(nameof(SetOrUpdateEndTime), activity =>
            {
                _appLogger.Info($"Starting {nameof(SetOrUpdateEndTime)} Parameter value = {endTime}. TraceID:{activity.TraceId}.");
            },
            activity =>
            {
                if (endTime == DateTime.MaxValue)
                {
                    _appLogger.Info($"EndTime parameter of {nameof(SetOrUpdateEndTime)} is set to MaxValue, indicating an unset or unrealistic time.");
                }
                else
                {
                    _currentCodingSession.EndTime = endTime;
                    _appLogger.Info($"EndTime set to {_currentCodingSession.EndTime}. TraceID: {activity.TraceId}.");
                }
            });
        }

        public void UpdateDurationSeconds(double durationSeconds)
        {
            _appLogger.LogActivity(nameof(UpdateDurationSeconds), activity =>
            {
                _appLogger.Info($"Starting {nameof(UpdateDurationSeconds)} with initial DurationSeconds = {_currentCodingSession.DurationSeconds}. TraceID: {activity.TraceId}");
            },
            activity =>
            {
                if (durationSeconds < 0)
                {
                    _appLogger.Error($"DurationSeconds parameter of {nameof(UpdateDurationSeconds)} is negative ({durationSeconds}).");
                }
                else
                {
                    _currentCodingSession.DurationSeconds = durationSeconds;
                    _appLogger.Info($"DurationSeconds set to {_currentCodingSession.DurationSeconds}. TraceID: {activity.TraceId}");
                }
            });
        }

        public void UpdateDurationHHMM(string durationHHMM)
        {
            _appLogger.LogActivity(nameof(UpdateDurationHHMM), activity =>
            {
                _appLogger.Info($"Starting {nameof(UpdateDurationHHMM)} with initial DurationHHMM = {_currentCodingSession.DurationHHMM}. TraceID: {activity.TraceId}");
            },
            activity =>
            {
                if (string.IsNullOrEmpty(durationHHMM) || !Regex.IsMatch(durationHHMM, @"^\d{2}:\d{2}$"))
                {
                    _appLogger.Error($"DurationHHMM parameter of {nameof(UpdateDurationHHMM)} is invalid or not set.");
                }
                else
                {
                    _currentCodingSession.DurationHHMM = durationHHMM;
                    _appLogger.Info($"DurationHHMM set to {_currentCodingSession.DurationHHMM}. TraceID: {activity.TraceId}");
                }
            });
        }

        public void UpdateGoalHHMM(string goalHHMM)
        {
            _appLogger.LogActivity(nameof(UpdateGoalHHMM), activity =>
            {
                _appLogger.Info($"Starting {nameof(UpdateGoalHHMM)} with initial GoalHHMM = {_currentCodingSession.GoalHHMM}. TraceID: {activity.TraceId}");
            },
            activity =>
            {
                if (string.IsNullOrEmpty(goalHHMM) || !Regex.IsMatch(goalHHMM, @"^\d{2}:\d{2}$"))
                {
                    _appLogger.Error($"GoalHHMM parameter of {nameof(UpdateGoalHHMM)} is invalid or not set.");
                }
                else
                {
                    _currentCodingSession.GoalHHMM = goalHHMM;
                    _appLogger.Info($"GoalHHMM set to {_currentCodingSession.GoalHHMM}. TraceID: {activity.TraceId}");
                }
            });
        }

        public void UpdateGoalReached(int goalReached)
        {
            _appLogger.LogActivity(nameof(UpdateGoalReached), activity =>
            {
                _appLogger.Info($"Starting {nameof(UpdateGoalReached)} with initial GoalReached = {_currentCodingSession.GoalReached}. TraceID: {activity.TraceId}");
            },
            activity =>
            {
                if (goalReached < 0 || goalReached > 1)  
                {
                    _appLogger.Error($"GoalReached parameter of {nameof(UpdateGoalReached)} is invalid ({goalReached}). Should be 0 or 1.");
                }
                else
                {
                    _currentCodingSession.GoalReached = goalReached;
                    _appLogger.Info($"GoalReached set to {_currentCodingSession.GoalReached}. TraceID: {activity.TraceId}");
                }
            });
        }



  

    }
}