using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.ICredentialManagers;
using System.Diagnostics;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.CodingSessions;
using CodingTracker.Common.IdGenerators;
using CodingTracker.Common.Interfaces.ICodingSessionRepository;
using System.Linq.Expressions;

namespace CodingTracker.Common.CodingSessionDTOManagers
{

    public interface ICodingSessionDTOManager
    {
        CodingSessionDTO CreateCodingSession();
        CodingSessionDTO GetCurrentSessionDTO();

        void SetSessionStartDate();
        void SetSessionStartTime();
        void SetSessionEndDate();
        void SetSessionEndTime();
        CodingSessionDTO GetOrCreateCurrentSessionDTO();
        CodingSessionDTO CreateAndReturnCurrentSessionDTO();

        string ConvertDurationSecondsIntoStringHHMM(double? durationSeconds);
        void UpdateCurrentSessionDTO(int sessionId, int userId, DateTime? startDate = null, DateTime? startTime = null, DateTime? endDate = null, DateTime? endTime = null, double? durationSeconds = null, string? durationHHMM = null, string? goalHHMM = null);
        TimeSpan ConvertDurationSecondsToTimeSpan(double? durationSeconds);

        string FormatTimeSpanToHHMM(TimeSpan timeSpan);


    }



    public class CodingSessionManager : ICodingSessionDTOManager
    {
        public CodingSession _currentCodingSession;

        private readonly IErrorHandler _errorHandler;
        private readonly IApplicationLogger _appLogger;
        private readonly ICredentialManager _credentialManager;
        private readonly IInputValidator _inputValidator;
        private readonly IIdGenerators _idGenerators;
        private readonly ICodingSessionRepository _codingSessionRepo;

        private bool IsCodingSessionActive = false;

        public CodingSessionManager(IErrorHandler errorHandler, IApplicationLogger appLogger, ICredentialManager credentialManager, IInputValidator inputValidator, IIdGenerators idGenerators, ICodingSessionRepository codingSessionRepo)
        {
            _errorHandler = errorHandler;
            _appLogger = appLogger;
            _credentialManager = credentialManager;
            _inputValidator = inputValidator;
            _idGenerators = idGenerators;
            _codingSessionRepo = codingSessionRepo;
        }

        public CodingSession ReturnCurrentCodingSession(Activity activity)
        {
            if(activity == null)
            {
                _appLogger.Error($"Error during {nameof(ReturnCurrentCodingSession)} activity = null");
            }
            _appLogger.Info($"Starting {nameof(ReturnCurrentCodingSession)} traceId: {activity.TraceId}.");
            return _currentCodingSession;
        }



        public CodingSession CreateNewCodingSession(int userId, Activity activity)
        {
            if (userId <= 0)
            {
                _appLogger.Error($"Invalid UserId: {userId} for {nameof(CreateNewCodingSession)}. UserId must be positive.");
                throw new ArgumentException($"UserId must be positive. Provided: {userId}", nameof(CreateNewCodingSession));
            }
            if (activity == null)
            {
                _appLogger.Error($"Activity is null for {nameof(CreateNewCodingSession)}.");
                throw new ArgumentNullException(nameof(activity), $"Activity cannot be null for {nameof(CreateNewCodingSession)}");
            }
            if (IsCodingSessionActive)
            {
                _appLogger.Error($"Cannot start a new coding session while another one is active. IsCodingSessionActive = {IsCodingSessionActive}. TraceId: {activity.TraceId}.");
                throw new InvalidOperationException($"Cannot start a new coding session while another one is active. TraceId: {activity.TraceId}");
            }

            try
            {
                _appLogger.Info($"Starting {nameof(CreateNewCodingSession)} TraceId: {activity.TraceId}.");

                int sessionId = _idGenerators.GenerateSessionId(activity);
                DateTime currentDateTime = DateTime.UtcNow;

                CodingSession codingSession = new CodingSession
                {
                    UserId = userId,
                    SessionId = sessionId,
                    StartDate = DateOnly.FromDateTime(currentDateTime),
                    StartTime = currentDateTime,
                };

                _appLogger.Info($"New coding session created. UserId: {codingSession.UserId}, SessionId: {codingSession.SessionId}, StartDate: {codingSession.StartDate}, StartTime: {codingSession.StartTime}. TraceId: {activity.TraceId}");

                return codingSession;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Critical error during {nameof(CreateNewCodingSession)}. TraceId: {activity.TraceId}", ex);
                throw;
            }
        }


        public void StartCodingSession(CodingSession codingSession, Activity activity, int userId)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity), $"Activity cannot be null for {nameof(StartCodingSession)}");
            if (codingSession == null) throw new ArgumentNullException(nameof(codingSession), $"codingSession cannot be null for {nameof(StartCodingSession)}");

            try
            {
                _appLogger.Info($"Starting {nameof(StartCodingSession)} TraceId: {activity.TraceId}.");

                CodingSession newSession = CreateNewCodingSession(userId, activity);

                IsCodingSessionActive = true;

                SetCurrentCodingSession(newSession, activity);

                _appLogger.Info($"Coding session started for {nameof(StartCodingSession)} TraceId: {activity.TraceId}.");
            }

            catch (Exception ex)
            {
                _appLogger.Error($"Critical error during {nameof(StartCodingSession)}", ex);
                throw;
            }
        }



        public void EndCodingSession(Activity activity)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity), $"Activity cannot be null for {nameof(EndCodingSession)}");

            try
            {
                _appLogger.Info($"Starting {nameof(EndCodingSession)} TraceId: {activity.TraceId}.");

                DateTime currentEndTime = DateTime.UtcNow;
                DateOnly currentendDate = DateOnly.FromDateTime(currentEndTime);
                int currentDurationSeconds = CalculateDurationSeconds(activity, _currentCodingSession.StartTime, currentEndTime);
                string currentDurationHHMM = ConvertDurationSecondsToStringHHMM(activity, currentDurationSeconds);
 

               
                _currentCodingSession.EndTime = currentEndTime;
                _currentCodingSession.EndDate = currentendDate;
                _currentCodingSession.DurationSeconds = currentDurationSeconds;
                _currentCodingSession.DurationHHMM = currentDurationHHMM;
            }
        }

        public string ConvertDurationSecondsToStringHHMM(Activity activity, int durationSeconds)
        {
            if (activity == null)
            {
                _appLogger.Error($"Activity is null for {nameof(ConvertDurationSecondsToStringHHMM)}");
                throw new ArgumentNullException(nameof(activity), $"Activity cannot be null for {nameof(ConvertDurationSecondsToStringHHMM)}");
            }
            if (durationSeconds < 0)
            {
                _appLogger.Error($"Negative duration provided: {durationSeconds} seconds. TraceId: {activity.TraceId}");
                throw new ArgumentOutOfRangeException(nameof(durationSeconds), $"DurationSeconds cannot be negative for {nameof(ConvertDurationSecondsToStringHHMM)}");
            }
            try
            {
                _appLogger.Info($"Starting {nameof(ConvertDurationSecondsToStringHHMM)} TraceId: {activity.TraceId}.");

                int hours = durationSeconds / 3600;
                int minutes = (durationSeconds % 3600) / 60;

                string formattedTime = $"{hours:D2}:{minutes:D2}";

                _appLogger.Info($"Converted {durationSeconds} seconds to {formattedTime} for {nameof(ConvertDurationSecondsToStringHHMM)} TraceId: {activity.TraceId}.");

                return formattedTime;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error in {nameof(ConvertDurationSecondsToStringHHMM)}. TraceId: {activity.TraceId}", ex);
                throw;
            }
        }

        public void UpdateCodingSessionEndTimes(Activity activity)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity), $"Activity cannot be null for {nameof(EndCodingSession)}");

            _appLogger.Info($"Starting {nameof(UpdateCodingSessionEndTimes)} TraceId: {activity.TraceId}");

            DateTime currentDateTime = DateTime.UtcNow;

            _currentCodingSession.EndDate = DateOnly.FromDateTime(currentDateTime);
            _currentCodingSession.EndTime = currentDateTime;


        }



               



        public void SetCurrentCodingSession(CodingSession codingSession, Activity activity)
        {
            if (activity == null) 
                throw new ArgumentNullException(nameof(activity), $"Activity cannot be null for {nameof(SetCurrentCodingSession)}");
            if (codingSession == null) 
                throw new ArgumentNullException(nameof(activity), $"codingSession cannot be null for {nameof(SetCurrentCodingSession)}");

            try
            {
                _appLogger.Info($"Starting {nameof(SetCurrentCodingSession)} TraceId: {activity.TraceId}.");

                _currentCodingSession = codingSession;


                // This is almost certainly not necessary but do not feel comfortable having a log indicating success without any checks.
                if(_currentCodingSession == codingSession)
                {
                    _appLogger.Info($" _currentCodingSession updated successfully TraceId: {activity.TraceId}.");
                }
                else
                {
                    _appLogger.Error($"Failed to set current coding session. TraceId: {activity.TraceId}");
                    throw new InvalidOperationException("Failed to set current coding session");
                }
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error in {nameof(SetCurrentCodingSession)}. TraceId: {activity.TraceId}", ex);
                throw;
            }
        }


        public int CalculateDurationSeconds(Activity activity, DateTime? startDate, DateTime? endDate)
        {
            if (activity == null)
            {
                _appLogger.Error($"Activity is null for {nameof(CalculateDurationSeconds)}");
                throw new ArgumentNullException(nameof(activity), $"Activity cannot be null for {nameof(CalculateDurationSeconds)}");
            }
            if (startDate == null)
            {
                _appLogger.Error($"StartDate is null for {nameof(CalculateDurationSeconds)}");
                throw new ArgumentNullException(nameof(startDate), $"StartDate cannot be null for {nameof(CalculateDurationSeconds)}");
            }
            if (endDate == null)
            {
                _appLogger.Error($"EndDate is null for {nameof(CalculateDurationSeconds)}");
                throw new ArgumentNullException(nameof(endDate), $"EndDate cannot be null for {nameof(CalculateDurationSeconds)}");
            }
            if (startDate >= endDate)
            {
                _appLogger.Error($"StartDate ({startDate:yyyy-MM-dd HH:mm:ss}) must be earlier than EndDate ({endDate:yyyy-MM-dd HH:mm:ss}) for {nameof(CalculateDurationSeconds)}.");
                throw new InvalidOperationException($"StartDate ({startDate:yyyy-MM-dd HH:mm:ss}) must be earlier than EndDate ({endDate:yyyy-MM-dd HH:mm:ss}) for {nameof(CalculateDurationSeconds)}.");
            }

            _appLogger.Info($"Starting {nameof(CalculateDurationSeconds)} TraceId: {activity.TraceId}.");

            int durationSeconds = (int)(endDate.Value - startDate.Value).TotalSeconds;

            _appLogger.Info($"durationSeconds calculated: {durationSeconds} TraceId: {activity.TraceId}.");

            return durationSeconds;
        }






























































        public void UpdateCurrentSessionDTO(int sessionId, int userId, DateTime? startDate = null,  DateTime? startTime = null,  DateTime? endDate = null, DateTime? endTime = null, double? durationSeconds = null, string? durationHHMM = null, string? goalHHMM = null)
        {
            using (var activity = new Activity(nameof(UpdateCurrentSessionDTO)).Start())
            {
                _appLogger.Info($"Starting {nameof(UpdateCurrentSessionDTO)}. TraceID: {activity.TraceId}");

                if (_currentCodingSession == null)
                {
                    _appLogger.Info("No current session DTO found. Creating new one.");
                    _currentCodingSession = CreateCodingSession();
                }

                _currentCodingSession.SessionId = sessionId;
                _currentCodingSession.UserId = userId;

                if (startDate.HasValue) 
                {
                    _currentCodingSession.StartDate = startDate.Value;
                }
                if (startTime.HasValue)
                {
                    _currentCodingSession.StartTime = startTime.Value;
                }
                if (endDate.HasValue) 
                {
                    _currentCodingSession.EndDate = endDate.Value;
                }
                if (endTime.HasValue)
                {
                    _currentCodingSession.EndTime = endTime.Value;
                }
                if (durationSeconds.HasValue)
                {
                    _currentCodingSession.DurationSeconds = durationSeconds.Value;
                }
                if (_inputValidator.IsValidTimeFormatHHMM(durationHHMM))
                {
                    _currentCodingSession.DurationHHMM = durationHHMM;
                }
                if(durationHHMM != null) 
                {
                    _currentCodingSession.DurationHHMM = durationHHMM;
                }
                if (goalHHMM != null) 
                {
                    _currentCodingSession.GoalHHMM = goalHHMM;
                }

                List<(string Name, object Value)> updates = new List<(string Name, object Value)>();

                updates.Add(("SessionId", (object)sessionId));
                updates.Add(("UserId", (object)userId));

                if (startDate.HasValue) updates.Add(("StartDate", (object)startDate));
                if (startTime.HasValue) updates.Add(("StartTime", (object)startTime));
                if (endDate.HasValue) updates.Add(("EndDate", ((object)endDate)));
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
