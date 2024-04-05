using System.Diagnostics;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.ICodingGoals;
using System.Linq.Expressions;
using CodingTracker.Common.ICodingSessions;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.ICodingSessionTimers;

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

        private CodingSessionDTO _currentSessionDTO;
 

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public bool IsStopWatchEnabled = false;
        private bool isCodingSessionActive = false;




        public CodingSession(IInputValidator validator, IApplicationLogger appLogger, ICodingGoal codingGoal, IErrorHandler errorHandler, ICodingSessionTimer sessionTimer)
        {
            _inputValidator = validator;
            _appLogger = appLogger;
            _codingGoal = codingGoal;
            _errorHandler = errorHandler;
            _sessionTimer = sessionTimer;

        }
        public CodingSessionDTO GetCurrentSessionDTO()
        {
            return _currentSessionDTO;
        }



        public void StartSession()
        {
            _errorHandler.CatchErrorsAndLogWithStopwatch(() =>
            {
                isCodingSessionActive = true;
               _sessionTimer.StartCodingSessionTimer();

                _currentSessionDTO = new CodingSessionDTO
                {
                    UserId =  _currentSessionDTO.UserId,
                    StartTime = _currentSessionDTO.StartTime,
                    StartDate = _currentSessionDTO.StartDate 
                };

                _appLogger.Info($"Session started, StartTime:");
            }, nameof(StartSession));
        }

        public void EndSession()
        {
            _errorHandler.CatchErrorsAndLogWithStopwatch(() =>
            {
                isCodingSessionActive = false;
                _sessionTimer.EndCodingSessionTimer();
                SetSessionTimeAndDate();


                _currentSessionDTO.DurationMinutes = CalculateDurationMinutes();

                _appLogger.Info($"Session ended, IsStopWatchEnabled: {IsStopWatchEnabled}, EndTime: {_currentSessionDTO.EndTime}.");
            }, nameof(EndSession));
        }

        public void SetSessionTimeAndDate()
        {
            _errorHandler.CatchErrorsAndLogWithStopwatch(() =>
            {
                DateTime endTime = DateTime.Now;
                _currentSessionDTO.EndTime = endTime;
                _currentSessionDTO.EndDate = endTime.Date; 

                _appLogger.Info($"End time and date set, EndTime: {endTime}, EndDate: {endTime.Date}");
            }, nameof(SetSessionTimeAndDate));
        }



        public bool CheckIfCodingSessionActive()
        {
            return _errorHandler.CatchErrorsAndLogWithStopwatch(() =>
            {
                bool isActive = isCodingSessionActive;
                _appLogger.Info($"Coding session active status: {isActive}");
                return isActive;
            }, nameof(CheckIfCodingSessionActive));
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
                        _appLogger.Error("StartCountDownTimer Time or End Time is not set.");
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



        public List<DateTime> GetDatesPrevious28days()
        {
            List<DateTime> dates = new List<DateTime>();
            DateTime today = DateTime.Today;
            for (int i = 1; i <= 28; i++)
            {
                dates.Add(today.AddDays(-i));
            }
            return dates;
        }
    }
}