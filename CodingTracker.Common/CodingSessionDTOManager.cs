using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.ICredentialManagers;
using System.Diagnostics;
using CodingTracker.Common.IDatabaseSessionReads;
using CodingTracker.Common.IInputValidators;

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

        string ConvertDurationSecondsIntoStringHHMM(int? durationSeconds);
        void UpdateCurrentSessionDTO(int sessionId, int userId, DateTime? startTime = null, DateTime? endTime = null, int? durationSeconds = null, string? durationHHMM = null, string? goalHHMM = null);
        TimeSpan ConvertDurationSecondsToTimeSpan(int? durationSeconds);

        string FormatTimeSpanToHHMM(TimeSpan timeSpan);

        List<string> ConvertSessionDurationsToHHMM(List<CodingSessionDTO> sessionDTOs);

    }



    public class CodingSessionDTOManager : ICodingSessionDTOManager
    {
        public CodingSessionDTO _currentSessionDTO;

        private readonly IErrorHandler _errorHandler;
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseSessionRead _databaseSessionRead;
        private readonly ICredentialManager _credentialManager;
        private readonly IInputValidator _inputValidator;

        private int _userId;
        private int _sessionId;
        private int _DurationHHMM;

        public CodingSessionDTOManager(IErrorHandler errorHandler, IApplicationLogger appLogger, IDatabaseSessionRead databaseSessionRead, ICredentialManager credentialManager, IInputValidator inputValidator)
        {
            _errorHandler = errorHandler;
            _appLogger = appLogger;
            _databaseSessionRead = databaseSessionRead;
            _credentialManager = credentialManager;
            _userId = _credentialManager.GetUserIdWithMostRecentLogin();
            _sessionId = _databaseSessionRead.GetSessionIdWithMostRecentLogin();
            _inputValidator = inputValidator;
        }

        public CodingSessionDTO GetCurrentSessionDTO()
        {
            using (var activity = new Activity(nameof(GetCurrentSessionDTO)).Start())
            {
                _appLogger.Debug($"Fetching current CodingSessionDTO. TraceID: {activity.TraceId}");
                return _currentSessionDTO;
            }
        }

        public CodingSessionDTO CreateCodingSessionDTO()
        {
            using (var activity = new Activity(nameof(CreateCodingSessionDTO)).Start())
            {
                var stopwatch = Stopwatch.StartNew();

                _currentSessionDTO = new CodingSessionDTO
                {
                    UserId = _userId,
                    SessionId = _sessionId
                };

                stopwatch.Stop();

                _appLogger.Info($"New CodingSessionDTO created with UserId: {_userId} and SessionId: {_sessionId}. Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");

                return _currentSessionDTO;
            }
        }

        public CodingSessionDTO GetOrCreateCurrentSessionDTO()
        {
            using (var activity = new Activity(nameof(GetOrCreateCurrentSessionDTO)).Start())
            {
                var stopwatch = Stopwatch.StartNew();

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

                stopwatch.Stop();

                _appLogger.Info($"GetOrCreateCurrentSessionDTO completed in {stopwatch.ElapsedMilliseconds}ms, TraceID: {activity.TraceId}");

                return _currentSessionDTO;

            }
        }

        public CodingSessionDTO CreateAndReturnCurrentSessionDTO()
        {
            using (var activity = new Activity(nameof(CreateAndReturnCurrentSessionDTO)).Start())
            {
                var stopwatch = Stopwatch.StartNew();

                _appLogger.Debug($"Starting {nameof(CreateAndReturnCurrentSessionDTO)}. TraceID: {activity.TraceId}");

                var dto = CreateCodingSessionDTO();

                stopwatch.Stop();

                _appLogger.Info($"New session DTO created successfully. Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");

                return dto;
            }
        }

        public void SetSessionStartTime()
        {
            using (var activity = new Activity(nameof(SetSessionStartTime)).Start())
            {
                var stopwatch = Stopwatch.StartNew();

                _appLogger.Info($"Starting {nameof(SetSessionStartTime)}. TraceID: {activity.TraceId}");

                try
                {
                    DateTime startTime = DateTime.Now;
                    UpdateCurrentSessionDTO(_currentSessionDTO.SessionId, _currentSessionDTO.UserId, startTime: startTime);
                    _appLogger.Info($"Start time set through UpdateCurrentSessionDTO, StartTime: {startTime}. Trace ID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(SetSessionStartTime)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }

                stopwatch.Stop();

                _appLogger.Info($"SetSessionStartTime completed in {stopwatch.ElapsedMilliseconds}ms, TraceID: {activity.TraceId}");
            }
        }


        public void SetSessionEndTime()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(SetSessionEndTime)).Start())
            {
                _appLogger.Info($"Starting {nameof(SetSessionEndTime)}. TraceID: {activity.TraceId}");

                try
                {
                    DateTime endTime = DateTime.Now;

                    UpdateCurrentSessionDTO(_currentSessionDTO.SessionId, _currentSessionDTO.UserId, endTime: endTime);
                    stopwatch.Stop();

                    _appLogger.Info($"End time set through UpdateCurrentSessionDTO, EndTime: {endTime}. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(SetSessionEndTime)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }

        public TimeSpan ConvertDurationSecondsToTimeSpan(int? durationSeconds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(ConvertDurationSecondsToTimeSpan)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ConvertDurationSecondsToTimeSpan)}. TraceID: {activity.TraceId}");

                if (!durationSeconds.HasValue)
                {
                    _appLogger.Debug("DurationSeconds has no value.");
                    stopwatch.Stop();
                    return TimeSpan.Zero;
                }

                TimeSpan result = TimeSpan.FromSeconds(durationSeconds.Value);

                stopwatch.Stop();

                _appLogger.Info($"Converted DurationSeconds to TimeSpan. Result: {result}. Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");
                return result;
            }
        }
        public string FormatTimeSpanToHHMM(TimeSpan timeSpan)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(FormatTimeSpanToHHMM)).Start())
            {
                _appLogger.Debug($"Starting {nameof(FormatTimeSpanToHHMM)}. TraceID: {activity.TraceId}");

                string formattedTime = timeSpan.ToString(@"hh\:mm");

                stopwatch.Stop();

                _appLogger.Info($"Formatted TimeSpan to HHMM: {formattedTime}. Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");
                return formattedTime;
            }
        }

        public string ConvertDurationSecondsIntoStringHHMM(int? durationSeconds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(ConvertDurationSecondsIntoStringHHMM)).Start())
            {
                _appLogger.Info($"Converting {durationSeconds} DurationSeconds into HH:MM. TraceID: {activity.TraceId}");
                try
                {
                    string result = FormatTimeSpanToHHMM(ConvertDurationSecondsToTimeSpan(durationSeconds));

                    stopwatch.Stop();

                    _appLogger.Info($"Conversion successful. HH:MM: {result}. Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");
                    return result;
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during conversion. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    stopwatch.Stop();
                    return null;
                }
            }
        }


        public List<string> ConvertSessionDurationsToHHMM(List<CodingSessionDTO> sessionDTOs)
        {
            List<string> formattedDurations = new List<string>();
            List<int> durationSecondsList = _databaseSessionRead.ReadSessionDurationSeconds(20);
            Stopwatch stopwatch = Stopwatch.StartNew();

            using (var activity = new Activity(nameof(ConvertSessionDurationsToHHMM)).Start())
            {
                _appLogger.Info($"Starting conversion of session durations to HH:MM. TraceID: {activity.TraceId}.");
                foreach (int seconds in durationSecondsList)
                {
                    try
                    {
                        string formattedDuration = ConvertDurationSecondsIntoStringHHMM(seconds);
                        formattedDurations.Add(formattedDuration);
                    }
                    catch (Exception ex)
                    {
                        _appLogger.Error($"Failed to convert {seconds} to HH:MM. Error: {ex.Message}. TraceID: {activity.TraceId}");
                        formattedDurations.Add("00:00");
                    }
                }
                stopwatch.Stop();
                _appLogger.Info($"Durations converted successfully. Execution time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
            }

            return formattedDurations;
        }



        public void UpdateCurrentSessionDTO(int sessionId, int userId, DateTime? startTime = null, DateTime? endTime = null, int? durationSeconds = null, string? durationHHMM = null, string? goalHHMM = null)
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
                if (_inputValidator.IsValidTimeFormatHHMM(durationHHMM))
                {
                    _currentSessionDTO.DurationHHMM = durationHHMM;
                }
                if (goalHHMM != null) 
                {
                    _currentSessionDTO.GoalHHMM = goalHHMM;
                }

                List<(string Name, object Value)> updates = new List<(string Name, object Value)>();

                updates.Add(("SessionId", (object)sessionId));
                updates.Add(("UserId", (object)userId));

                if (startTime.HasValue) updates.Add(("StartTime", (object)startTime));
                if (endTime.HasValue) updates.Add(("EndTime", (object)endTime));
                if (durationSeconds.HasValue) updates.Add(("DurationSeconds", (object)durationSeconds));
                if (durationHHMM != null) updates.Add(("DurationHHMM", (object)durationHHMM));
                if (goalHHMM !=  null) updates.Add(("goalHHMM", (object)goalHHMM));

                _appLogger.LogUpdates(nameof(UpdateCurrentSessionDTO), updates.ToArray());

                _appLogger.Info($"Updated {nameof(UpdateCurrentSessionDTO)} successfully. TraceID: {activity.TraceId}");
            }
        }
    }
}
