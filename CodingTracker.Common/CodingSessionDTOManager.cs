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

        void SetSessionStartDate();
        void SetSessionStartTime();
        void SetSessionEndDate();
        void SetSessionEndTime();
        CodingSessionDTO GetOrCreateCurrentSessionDTO();
        CodingSessionDTO CreateAndReturnCurrentSessionDTO();

        string ConvertDurationSecondsIntoStringHHMM(double? durationSeconds);
        void UpdateCurrentSessionDTO(int sessionId, int userId, DateTime? startDate = null, DateTime? startTime = null, DateTime? endDate = null, DateTime? endTime = null, double? durationSeconds = null, string? durationHHMM = null, string? goalHHMM = null, int goalReached = 0);
        TimeSpan ConvertDurationSecondsToTimeSpan(double? durationSeconds);

        string FormatTimeSpanToHHMM(TimeSpan timeSpan);

        List<string> SessionDurationSecondsToHHMM(List<CodingSessionDTO> sessionDTOs);

        void SetCodingSessionUserId(int userId);

    }



    public class CodingSessionDTOManager : ICodingSessionDTOManager
    {
        private CodingSessionDTO _currentSessionDTO;

        private readonly IErrorHandler _errorHandler;
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseSessionRead _databaseSessionRead;
        private readonly ICredentialManager _credentialManager;
        private readonly IInputValidator _inputValidator;

        private int _userId;
        private int CurrentSessionUserId;
        private int _DurationHHMM;

        public CodingSessionDTOManager(IErrorHandler errorHandler, IApplicationLogger appLogger, IDatabaseSessionRead databaseSessionRead, ICredentialManager credentialManager, IInputValidator inputValidator)
        {
            _errorHandler = errorHandler;
            _appLogger = appLogger;
            _databaseSessionRead = databaseSessionRead;
            _credentialManager = credentialManager;
            CurrentSessionUserId = _databaseSessionRead.GetSessionIdWithMostRecentLogin();
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
                _appLogger.Debug($"Starting {nameof(CreateCodingSessionDTO)}, TraceId: {activity.TraceId}.");
                var stopwatch = Stopwatch.StartNew();

                _currentSessionDTO = new CodingSessionDTO
                {
                    userId = _userId,
                    sessionId = CurrentSessionUserId,
                };

                stopwatch.Stop();

                _appLogger.Info($"New CodingSessionDTO created with userId: {_userId} and sessionId: {CurrentSessionUserId}. Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");

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
                Stopwatch stopwatch = Stopwatch.StartNew();

                _appLogger.Debug($"Starting {nameof(CreateAndReturnCurrentSessionDTO)}. TraceID: {activity.TraceId}");

                var dto = CreateCodingSessionDTO();

                stopwatch.Stop();

                _appLogger.Info($"New session DTO created successfully. Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");

                return dto;
            }
        }

        public void SetSessionStartDate()
        {
            using (var activity = new Activity(nameof(SetSessionStartDate)).Start())
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                _appLogger.Info($"Starting {nameof(SetSessionStartDate)}. TraceID: {activity.TraceId}");

                try
                {
                    DateTime startDate = DateTime.Today; // Use DateTime.Today to get the current date with the time component set to 00:00:00.
                    UpdateCurrentSessionDTO(_currentSessionDTO.sessionId, _currentSessionDTO.userId, startDate: startDate);
                    _appLogger.Info($"Start date set through UpdateCurrentSessionDTO, startDate: {startDate}. Trace ID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(SetSessionStartDate)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }

                stopwatch.Stop();

                _appLogger.Info($"SetSessionStartDate completed in {stopwatch.ElapsedMilliseconds}ms, TraceID: {activity.TraceId}");
            }
        }

        public void SetSessionStartTime()
        {
            using (var activity = new Activity(nameof(SetSessionStartTime)).Start())
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                _appLogger.Info($"Starting {nameof(SetSessionStartTime)}. TraceID: {activity.TraceId}");

                try
                {
                    DateTime startTime = DateTime.Now;
                    UpdateCurrentSessionDTO(_currentSessionDTO.sessionId, _currentSessionDTO.userId, startTime: startTime);
                    _appLogger.Info($"Start time set through UpdateCurrentSessionDTO, startTime: {startTime}. Trace ID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(SetSessionStartTime)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }

                stopwatch.Stop();

                _appLogger.Info($"SetSessionStartTime completed in {stopwatch.ElapsedMilliseconds}ms, TraceID: {activity.TraceId}");
            }
        }

        public void SetSessionEndDate()
        {
            using (var activity = new Activity(nameof(SetSessionEndDate)).Start())
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                _appLogger.Info($"Starting {nameof(SetSessionEndDate)}. TraceID: {activity.TraceId}");

                try
                {
                    DateTime endDate = DateTime.Today; // Using DateTime.Today to get the current date without the time part.
                    UpdateCurrentSessionDTO(_currentSessionDTO.sessionId, _currentSessionDTO.userId, endDate: endDate);
                    _appLogger.Info($"End date set through UpdateCurrentSessionDTO, endDate: {endDate}. Trace ID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(SetSessionEndDate)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }

                stopwatch.Stop();

                _appLogger.Info($"SetSessionEndDate completed in {stopwatch.ElapsedMilliseconds}ms, TraceID: {activity.TraceId}");
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

                    UpdateCurrentSessionDTO(_currentSessionDTO.sessionId, _currentSessionDTO.userId, endTime: endTime);
                    stopwatch.Stop();

                    _appLogger.Info($"End time set through UpdateCurrentSessionDTO, endTime: {endTime}. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(SetSessionEndTime)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }

        public TimeSpan ConvertDurationSecondsToTimeSpan(double? durationSeconds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(ConvertDurationSecondsToTimeSpan)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ConvertDurationSecondsToTimeSpan)}. TraceID: {activity.TraceId}");

                if (!durationSeconds.HasValue)
                {
                    _appLogger.Debug("durationSeconds has no value.");
                    stopwatch.Stop();
                    return TimeSpan.Zero;
                }

                TimeSpan result = TimeSpan.FromSeconds(durationSeconds.Value);

                stopwatch.Stop();

                _appLogger.Info($"Converted durationSeconds to TimeSpan. Result: {result}. Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");
                return result;
            }
        }

        public string ConvertDurationSecondsIntoStringHHMM(double? durationSeconds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(ConvertDurationSecondsIntoStringHHMM)).Start())
            {
                _appLogger.Info($"Converting {durationSeconds} durationSeconds into HH:MM. TraceID: {activity.TraceId}");
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


        public List<string> SessionDurationSecondsToHHMM(List<CodingSessionDTO> sessionDTOs)
        {
            List<string> formattedDurations = new List<string>();
            List<double> durationSecondsList = _databaseSessionRead.ReadSessionDurationSeconds(20);
            Stopwatch stopwatch = Stopwatch.StartNew();

            using (var activity = new Activity(nameof(SessionDurationSecondsToHHMM)).Start())
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



        public void UpdateCurrentSessionDTO(int sessionId, int userId, DateTime? startDate = null,  DateTime? startTime = null,  DateTime? endDate = null, DateTime? endTime = null, double? durationSeconds = null, string? durationHHMM = null, string? goalHHMM = null, int goalReached = 0)
        {
            using (var activity = new Activity(nameof(UpdateCurrentSessionDTO)).Start())
            {
                _appLogger.Info($"Starting {nameof(UpdateCurrentSessionDTO)}. TraceID: {activity.TraceId}");

                if (_currentSessionDTO == null)
                {
                    _appLogger.Info("No current session DTO found. Creating new one.");
                    _currentSessionDTO = CreateCodingSessionDTO();
                }

                _currentSessionDTO.sessionId = sessionId;
                _currentSessionDTO.userId = userId;

                if (startDate.HasValue) 
                {
                    _currentSessionDTO.startDate = startDate.Value;
                }
                if (startTime.HasValue)
                {
                    _currentSessionDTO.startTime = startTime.Value;
                }
                if (endDate.HasValue) 
                {
                    _currentSessionDTO.endDate = endDate.Value;
                }
                if (endTime.HasValue)
                {
                    _currentSessionDTO.endTime = endTime.Value;
                }
                if (durationSeconds.HasValue)
                {
                    _currentSessionDTO.durationSeconds = durationSeconds.Value;
                }
                if (_inputValidator.IsValidTimeFormatHHMM(durationHHMM))
                {
                    _currentSessionDTO.durationHHMM = durationHHMM;
                }
                if(durationHHMM != null) 
                {
                    _currentSessionDTO.durationHHMM = durationHHMM;
                }
                if (goalHHMM != null) 
                {
                    _currentSessionDTO.goalHHMM = goalHHMM;
                }

                List<(string Name, object Value)> updates = new List<(string Name, object Value)>();

                updates.Add(("sessionId", (object)sessionId));
                updates.Add(("userId", (object)userId));

                if (startDate.HasValue) updates.Add(("startDate", (object)startDate));
                if (startTime.HasValue) updates.Add(("startTime", (object)startTime));
                if (endDate.HasValue) updates.Add(("endDate", ((object)endDate)));
                if (endTime.HasValue) updates.Add(("endTime", (object)endTime));
                if (durationSeconds.HasValue) updates.Add(("durationSeconds", (object)durationSeconds));
                if (durationHHMM != null) updates.Add(("durationHHMM", (object)durationHHMM));
                if (goalHHMM !=  null) updates.Add(("goalHHMM", (object)goalHHMM));

                _appLogger.LogUpdates(nameof(UpdateCurrentSessionDTO), updates.ToArray());

                _appLogger.Info($"Updated {nameof(UpdateCurrentSessionDTO)} successfully. _currentSessionDTOUserId: {_currentSessionDTO.userId} TraceID: {activity.TraceId}");
            }
        }

        public void SetCodingSessionUserId(int userId)
        {
            _appLogger.LogActivity(nameof(SetCodingSessionUserId), activity =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                _appLogger.Info($"Starting {nameof(SetCodingSessionUserId)}, TraceID:{activity.TraceId}.");
            },
            activity =>
            {

                if (userId > 0)
                {
                    CurrentSessionUserId = userId;
                    _appLogger.Info($"CodingSessionUserId set to {userId} TraceID:{activity.TraceId}");
                }
                else
                {
                    _appLogger.Info($"CurrentSessionUserID is not greater than 0 (0 is default for not set) TraceID:{activity.TraceId}");
                }
            },
            activity =>
            {
                _appLogger.Info($"{nameof(SetCodingSessionUserId)} complete TraceID:{activity.TraceId}");
            });
        }
    }
}