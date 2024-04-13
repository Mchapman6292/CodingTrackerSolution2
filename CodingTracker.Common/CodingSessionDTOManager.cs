using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.ICredentialManagers;
using System.Diagnostics;
using CodingTracker.Common.IDatabaseSessionReads;

namespace CodingTracker.Common.CodingSessionDTOManagers
{

    public interface ICodingSessionDTOManager
    {
        CodingSessionDTO CreateCodingSessionDTO();
        CodingSessionDTO GetCurrentSessionDTO();
        void SetSessionStartTime();
        void SetSessionEndTime();
        CodingSessionDTO GetOrCreateCurrentSessionDTO();
        CodingSessionDTO CreateAndReturnCurrentSessionDTO();
        void UpdateCurrentSessionDTO(int sessionId, int userId, DateTime? startTime = null, DateTime? endTime = null, int? durationSeconds = null);
    }



    public class CodingSessionDTOManager : ICodingSessionDTOManager
    {
        public CodingSessionDTO _currentSessionDTO;

        private readonly IErrorHandler _errorHandler;
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseSessionRead _databaseSessionRead;
        private readonly ICredentialManager _credentialManager;

        private int _userId;
        private int _sessionId;

        public CodingSessionDTOManager(IErrorHandler errorHandler, IApplicationLogger appLogger, IDatabaseSessionRead databaseSessionRead, ICredentialManager credentialManager)
        {
            _errorHandler = errorHandler;
            _appLogger = appLogger;
            _databaseSessionRead = databaseSessionRead;
            _credentialManager = credentialManager;
            _userId = _credentialManager.GetUserIdWithMostRecentLogin();
            _sessionId = _databaseSessionRead.GetSessionIdWithMostRecentLogin();

        }

        public CodingSessionDTO GetCurrentSessionDTO()
        {
            return _currentSessionDTO;
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


        public void SetSessionStartTime()
        {
            using (var activity = new Activity(nameof(SetSessionStartTime)).Start())
            {
                _appLogger.Info($"Starting {nameof(SetSessionStartTime)}. TraceID: {activity.TraceId}");

                try
                {
                    DateTime startTime = DateTime.Now;
                    UpdateCurrentSessionDTO(_currentSessionDTO.SessionId, _currentSessionDTO.UserId, startTime: startTime);

                    _appLogger.Info($"Start time set through UpdateCurrentSessionDTO, StartTime: {startTime}. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(SetSessionStartTime)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }


        public void SetSessionEndTime()
        {
            using (var activity = new Activity(nameof(SetSessionEndTime)).Start())
            {
                _appLogger.Info($"Starting {nameof(SetSessionEndTime)}. TraceID: {activity.TraceId}");

                try
                {
                    DateTime endTime = DateTime.Now;

                    UpdateCurrentSessionDTO(_currentSessionDTO.SessionId, _currentSessionDTO.UserId, endTime: endTime);

                    _appLogger.Info($"End time set through UpdateCurrentSessionDTO, EndTime: {endTime}. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(SetSessionEndTime)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }


        public void UpdateCurrentSessionDTO(int sessionId, int userId, DateTime? startTime = null, DateTime? endTime = null, int? durationSeconds = null)
        {
            using (var activity = new Activity(nameof(UpdateCurrentSessionDTO)).Start())
            {
                _appLogger.Info($"Starting {nameof(UpdateCurrentSessionDTO)}. TraceID: {activity.TraceId}");

                if (_currentSessionDTO == null)
                {
                    _appLogger.Info("No current session DTO found. Creating new one.");
                    _currentSessionDTO = CreateCodingSessionDTO();
                }

                _currentSessionDTO.SessionId = sessionId;
                _currentSessionDTO.UserId = userId;

                if (startTime.HasValue)
                {
                    _currentSessionDTO.StartTime = startTime.Value;
                }
                if (endTime.HasValue)
                {
                    _currentSessionDTO.EndTime = endTime.Value;
                }
                if (durationSeconds.HasValue)
                {
                    _currentSessionDTO.DurationSeconds = durationSeconds.Value;
                }

                List<(string Name, object Value)> updates = new List<(string Name, object Value)>();
                updates.Add(("SessionId", (object)sessionId));
                updates.Add(("UserId", (object)userId));
                if (startTime.HasValue) updates.Add(("StartTime", (object)startTime));
                if (endTime.HasValue) updates.Add(("EndTime", (object)endTime));
                if (durationSeconds.HasValue) updates.Add(("DurationSeconds", (object)durationSeconds));

                _appLogger.LogUpdates(nameof(UpdateCurrentSessionDTO), updates.ToArray());

                _appLogger.Info($"Updated {nameof(UpdateCurrentSessionDTO)} successfully. TraceID: {activity.TraceId}");
            }
        }
    }
}
