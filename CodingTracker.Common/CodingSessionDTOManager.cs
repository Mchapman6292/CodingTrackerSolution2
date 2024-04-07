using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.CodingSessionDTOs;
using System.Diagnostics;
using CodingTracker.Common.ICodingSessionDTOProviders;
using CodingTracker.Common.IDatabaseSessionReads;

namespace CodingTracker.Common.CodingSessionDTOProviders
{
    public class CodingSessionDTOManager : ICodingSessionDTOManager
    {
        public CodingSessionDTO _currentSessionDTO;

        private readonly IErrorHandler _errorHandler;
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseSessionRead _databaseSessionRead;
        private int _userId;
        private int _sessionId;

        public CodingSessionDTOManager(IErrorHandler errorHandler, IApplicationLogger appLogger, IDatabaseSessionRead databaseSessionRead, int userID, int sessionID)
        {
            _errorHandler = errorHandler;
            _appLogger = appLogger;
            _databaseSessionRead = databaseSessionRead;
            userID = _databaseSessionRead.GetUserIdWithMostRecentLogin();
            sessionID = _databaseSessionRead.GetSessionIdWithMostRecentLogin();
        }

        public CodingSessionDTO CreateCodingSessionDTO() // Creates a new CodingSessionDTO, assigns it to _currentSessionDTO, overwriting any existing session DTO in the provider.
        {
            using (var activity = new Activity(nameof(CreateCodingSessionDTO)).Start())
            {
                _currentSessionDTO = new CodingSessionDTO
                {
                    UserId = _userId,
                    SessionId = _sessionId
                };

                _appLogger.Info($"New CodingSessionDTO created with UserId: {_userId} and SessionId: {_sessionId}. TraceID: {activity.TraceId}");
            }
            return _currentSessionDTO;
        }


            public CodingSessionDTO GetOrCreateCurrentSessionDTO() // Returns the existing session DTO if available, or creates a new one if not.
        {
            using (var activity = new Activity(nameof(GetOrCreateCurrentSessionDTO)).Start())
            {
                _appLogger.Debug($"Starting {nameof(GetOrCreateCurrentSessionDTO)}. TraceID: {activity.TraceId}");

                if (_currentSessionDTO == null)
                {
                    _appLogger.Info($"No current session DTO found. Creating new one. TraceID: {activity.TraceId}");
                    CreateCodingSessionDTO();
                }
                else
                {
                    _appLogger.Info($"Returning existing session DTO. TraceID: {activity.TraceId}");
                }

            return _currentSessionDTO;
            }
        }


        public CodingSessionDTO CreateAndReturnCurrentSessionDTO() // creates and returns a new session DTO, regardless of whether an existing DTO is present,                                                     
        {
            using (var activity = new Activity(nameof(CreateAndReturnCurrentSessionDTO)).Start())
            {
                {
                    _appLogger.Debug($"Starting {nameof(CreateAndReturnCurrentSessionDTO)}. TraceID: {activity.TraceId}");
                    var dto = CreateCodingSessionDTO();
                    _appLogger.Info($"New session DTO created successfully. TraceID: {activity.TraceId}");

                    return dto;
                }
            }
        }

        public void SetSessionEndTimeAndDate()
        {
            _errorHandler.CatchErrorsAndLogWithStopwatch(() =>
            {
                DateTime endTime = DateTime.Now;
                _currentSessionDTO.EndTime = endTime;
                _currentSessionDTO.EndDate = endTime.Date;

                _appLogger.Info($"End time and date set, EndTime: {endTime}, EndDate: {endTime.Date}");
            }, nameof(SetSessionEndTimeAndDate));
        }

        public void SetSessionStartTimeAndDate()
        {
            using (var activity = new Activity(nameof(SetSessionStartTimeAndDate)).Start())
            {
                DateTime startTime = DateTime.Now;
                _currentSessionDTO.StartTime = startTime;
                _currentSessionDTO.StartDate = startTime.Date;
            }
        }

        public int CalculateDurationMinutes()
        {
            int durationMins = 0;
            using (var activity = new Activity(nameof(CalculateDurationMinutes)).Start())
            {
                _appLogger.Info($"Calculating duration minutes. TraceID: {activity.TraceId}");

                try
                {
                    if (!_currentSessionDTO.StartTime.HasValue || !_currentSessionDTO.EndTime.HasValue)
                    {
                        _appLogger.Error("start Time or End Time is not set.");
                    }

                    TimeSpan duration = _currentSessionDTO.EndTime.Value - _currentSessionDTO.StartTime.Value;
                    durationMins = (int)duration.TotalMinutes;

                    _appLogger.Info($"Duration minutes calculated. TraceID: {activity.TraceId}, DurationMinutes: {_currentSessionDTO.DurationMinutes}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(CalculateDurationMinutes)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
            return durationMins;
        }

        public void UpdateCurrentSessionDTO(int sessionId, int userId , DateTime? startTime =null, DateTime? endTime =null, DateTime? startDate =null, DateTime? endDate =null, int? durationMinutes =null)

        {
            using (var activity = new Activity(nameof(UpdateCurrentSessionDTO)).Start())


                _currentSessionDTO.SessionId = sessionId;
                _currentSessionDTO.UserId = userId;
                _currentSessionDTO.StartTime = startTime;
                _currentSessionDTO.EndTime = endTime;
                _currentSessionDTO.StartDate = startDate;
                _currentSessionDTO.EndDate = endDate;
                _currentSessionDTO.DurationMinutes = durationMinutes;

                _appLogger.LogUpdates(nameof(UpdateCurrentSessionDTO),
                ("SessionId", (object)sessionId),
                ("UserId", (object)userId),
                ("StartTime", (object)startTime), 
                ("EndTime", (object?)endTime),    // Nullable DateTime, cast to object?
                ("StartDate", (object?)startDate),
                ("EndDate", (object?)endDate),
                ("DurationMinutes", (object?)durationMinutes));
        }
    }
}
