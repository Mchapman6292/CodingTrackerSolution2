using System.Diagnostics;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.ICodingGoals;
using System.Linq.Expressions;
using CodingTracker.Common.ICodingSessions;

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
        private CodingSessionDTO _currentSessionDTO;
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DurationMinutes { get; set; }
        public int? CodingGoalHours { get; set; }
        public int? TimeToGoalMinutes { get; set; }
        public int? SessionGoalMinutes { get; set; }

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public bool IsStopWatchEnabled = false;
        private bool isCodingSessionActive = false;




        public CodingSession(IInputValidator validator, IApplicationLogger appLogger, ICodingGoal codingGoal)
        {
            _inputValidator = validator;
            _appLogger = appLogger;
            _codingGoal = codingGoal;

        }
        public CodingSessionDTO GetCurrentSessionDTO()
        {
            return _currentSessionDTO;
        }

        public void StartSession(int userId)
        {
            using (var activity = new Activity(nameof(StartSession)).Start())
            {
                _appLogger.Info($"Starting {nameof(StartSession)}. TraceID: {activity.TraceId}");
                try
                {
                    UserId = userId; 
                    DateTime startTime = DateTime.Now;

                    if (IsStopWatchEnabled)
                    {
                        _stopwatch.Start();
                    }

                    _currentSessionDTO = new CodingSessionDTO
                    {
                        UserId = this.UserId,
                        StartTime = startTime,
                        StartDate = startTime.Date 
                    };

                    _appLogger.Info($"Session started for UserId: {UserId}. TraceID: {activity.TraceId}, StartTime: {startTime}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(StartSession)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }

        public void EndSession()
        {
            using (var activity = new Activity(nameof(EndSession)).Start())
            {
                _appLogger.Info($"Ending {nameof(EndSession)}. TraceID: {activity.TraceId}");
                try
                {
                    if (IsStopWatchEnabled)
                    {
                        _stopwatch.Stop();
                    }
                    else
                    {
                        EndTime = DateTime.Now;
                    }
                    CalculateDurationMinutes();
                    CalculateTimeToGoal();

                    var codingSessionDTO = new CodingSessionDTO
                    {
                        SessionId = this.SessionId,
                        UserId = this.UserId,
                        StartTime = this.StartTime,
                        EndTime = this.EndTime,
                        StartDate = this.StartDate,
                        EndDate = this.EndDate,
                        DurationMinutes = this.DurationMinutes,
                        CodingGoalHours = this.CodingGoalHours,
                        TimeToGoalMinutes = this.TimeToGoalMinutes ?? 0,
                    };
                    _appLogger.Info($"Session ended. TraceID: {activity.TraceId}, IsStopWatchEnabled: {IsStopWatchEnabled}, EndTime: {EndTime}, DurationMinutes: {DurationMinutes}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(EndSession)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }

        public void SaveCurrentCodingSession()
        {
            using (var activity = new Activity(nameof(SaveCurrentCodingSession)).Start())
            {
                _appLogger.Info($"Saving current coding session. TraceID: {activity.TraceId}");
                try
                {
                    if (!StartTime.HasValue)
                    {
                        throw new InvalidOperationException("Session start time is not set.");
                    }

                    EndTime ??= DateTime.UtcNow;

                    if (!DurationMinutes.HasValue && EndTime.HasValue)
                    {
                        DurationMinutes = (int)(EndTime.Value - StartTime.Value).TotalMinutes;
                    }

                    _currentSessionDTO = new CodingSessionDTO
                    {
                        SessionId = this.SessionId,
                        UserId = this.UserId,
                        StartTime = this.StartTime,
                        EndTime = this.EndTime,
                        StartDate = this.StartDate ?? this.StartTime?.Date,
                        EndDate = this.EndDate ?? this.EndTime?.Date,
                        DurationMinutes = this.DurationMinutes,
                        CodingGoalHours = this.CodingGoalHours,
                        TimeToGoalMinutes = this.TimeToGoalMinutes ?? 0
                    };

                    _appLogger.Info($"Current coding session saved. TraceID: {activity.TraceId}, SessionId: {SessionId}, EndTime: {EndTime}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(SaveCurrentCodingSession)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }


        public bool CheckIfCodingSessionActive()
        {
            using var activity = new Activity(nameof(CheckIfCodingSessionActive)).Start();
            _appLogger.Info($"Checking if coding session is active. TraceID: {activity.TraceId})");
            try
            {
                bool isActive = isCodingSessionActive;
                _appLogger.Info($"Coding session active status: {isActive}. TraceID: {activity.TraceId}");
                return isActive;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"An error occurred during {nameof(CheckIfCodingSessionActive)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
            }
            return isCodingSessionActive;
        }


        public void SetStartTimeManually()
        {
            using (var activity = new Activity(nameof(SetStartTimeManually)).Start())
            {
                _appLogger.Info($"Starting {nameof(SetStartTimeManually)}. TraceID: {activity.TraceId}");
                try
                {
                    StartDate = _inputValidator.GetValidDateFromUser();
                    StartTime = _inputValidator.GetValidTimeFromUser();

                    _appLogger.Info($"StartCountDownTimer time manually set. TraceID: {activity.TraceId}, StartDate: {StartDate}, StartTime: {StartTime}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(SetStartTimeManually)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }
        public void SetEndTimeManually()
        {
            using (var activity = new Activity(nameof(SetEndTimeManually)).Start())
            {
                _appLogger.Info($"Starting {nameof(SetEndTimeManually)}. TraceID: {activity.TraceId}");
                try
                {
                    EndDate = _inputValidator.GetValidDateFromUser();
                    EndTime = _inputValidator.GetValidTimeFromUser();

                    _appLogger.Info($"End time manually set. TraceID: {activity.TraceId}, EndDate: {EndDate}, EndTime: {EndTime}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(SetEndTimeManually)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }

        public void CalculateDurationMinutes()
        {
            using (var activity = new Activity(nameof(CalculateDurationMinutes)).Start())
            {
                _appLogger.Info($"Calculating duration minutes. TraceID: {activity.TraceId}");

                try
                {
                    if (!StartTime.HasValue || !EndTime.HasValue)
                    {
                        throw new InvalidOperationException("StartCountDownTimer Time or End Time is not set.");
                    }

                    TimeSpan duration = EndTime.Value - StartTime.Value;
                    DurationMinutes = (int)duration.TotalMinutes;

                    _appLogger.Info($"Duration minutes calculated. TraceID: {activity.TraceId}, DurationMinutes: {DurationMinutes}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(CalculateDurationMinutes)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }

        public bool CheckBothDurationCalculations()
        {
            using (var activity = new Activity(nameof(CheckBothDurationCalculations)).Start())
            {
                _appLogger.Info($"Checking both duration calculations. TraceID: {activity.TraceId}");

                try
                {
                    if (!IsStopWatchEnabled || !StartTime.HasValue || !EndTime.HasValue)
                    {
                        throw new InvalidOperationException("Cannot check durations - either stopwatch is not enabled or manual times are not set.");
                    }

                    int stopwatchMinutes = (int)_stopwatch.Elapsed.TotalMinutes;
                    int manualDurationMinutes = (int)((EndTime.Value - StartTime.Value).TotalMinutes);

                    bool areDurationsEqual = stopwatchMinutes == manualDurationMinutes;

                    _appLogger.Info($"Both duration calculations checked. TraceID: {activity.TraceId}, StopwatchMinutes: {stopwatchMinutes}, ManualDurationMinutes: {manualDurationMinutes}, AreDurationsEqual: {areDurationsEqual}");

                    return areDurationsEqual;
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(CheckBothDurationCalculations)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    return false;
                }
            }
        }

        public void CalculateTimeToGoal() // This should belong in CodingGoal but moving here as a temporary fix for circular dependencies. 
        {
            using (var activity = new Activity(nameof(CalculateTimeToGoal)).Start())
            {
                _appLogger.Info($"Starting {nameof(CalculateTimeToGoal)}. TraceID: {activity.TraceId}");
                try
                {
                    if (!CodingGoalHours.HasValue || CodingGoalHours.Value <= 0)
                    {
                        throw new InvalidOperationException("Coding goal must be set and greater than zero.");
                    }
                    if (!DurationMinutes.HasValue)
                    {
                        throw new InvalidOperationException("Session duration is not set.");
                    }

                    int sessionDurationMinutes = DurationMinutes.Value;

                    if (TimeToGoalMinutes.HasValue && TimeToGoalMinutes.Value < sessionDurationMinutes)
                    {
                        throw new InvalidOperationException("Session duration exceeds the remaining time to goal.");
                    }

                    if (!TimeToGoalMinutes.HasValue)
                    {
                        TimeToGoalMinutes = CodingGoalHours.Value * 60;
                    }

                    TimeToGoalMinutes -= sessionDurationMinutes;

                    _appLogger.Info($"Time to goal calculated. TraceID: {activity.TraceId}, TimeToGoalMinutes: {TimeToGoalMinutes}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(CalculateTimeToGoal)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }
    }
}