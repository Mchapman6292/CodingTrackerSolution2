using System.Diagnostics;
using CodingTracker.Common.IInputValidators;

// method to record start & end time
// logic to hold recorded times & view them
// user should be able to input start & end times manually 

namespace CodingTracker.Business.CodingSession
{
    public class CodingSession
    {
        private readonly IInputValidator _inputValidator;
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DurationMinutes { get; set; }
        public int? CodingGoalHours { get; set; }
        public int? TimeToGoalMinutes { get; set; }
        public string? SessionNotes { get; set; }

        private readonly Stopwatch _stopwatch = new Stopwatch();
        public bool IsStopWatchEnabled = false;



        public CodingSession(IInputValidator validator)
        {
            _inputValidator = validator;

        }
        public void StartSession()
        {
            if (IsStopWatchEnabled)
            {
                _stopwatch.Start();
            }
            else
            {
                StartTime = DateTime.Now;
            }

        }

        public void EndSession()
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
            CalculateGoalProgress();
        }

        public string FormatTimeToGoalToHHMM(int? timeToGoal)
        {
            if (!timeToGoal.HasValue)
            {
                throw new ArgumentException("Minutes value is required.");
            }

            int totalMinutes = timeToGoal.Value;
            int hours = totalMinutes / 60;
            int remainingMinutes = totalMinutes % 60;

            return $"{hours:D2}:{remainingMinutes:D2}";
        }


        public void SetCodingGoal(int goalHours)
        {
            CodingGoalHours = goalHours;

            TimeToGoalMinutes = (CodingGoalHours * 60);
        }

        public void CalculateGoalProgress()
        {
            if (!CodingGoalHours.HasValue || CodingGoalHours.Value <= 0)
            {
                throw new InvalidOperationException("Coding goal must be set and greater than zero.");
            }
            CalculateDurationMinutes();
            if (!DurationMinutes.HasValue)
            {
                throw new InvalidOperationException("Session duration could not be determined.");
            }

            int goalMinutes = CodingGoalHours.Value * 60;


            if (DurationMinutes.Value >= goalMinutes)
            {
                TimeToGoalMinutes = 0;
            }
            else
            {

                TimeToGoalMinutes = goalMinutes - DurationMinutes.Value;
            }
        }

        public void SetStartTimeManually()
            {
                StartDate = _inputValidator.GetValidDateFromUser();
                StartTime = _inputValidator.GetValidTimeFromUser();
            }

            public void SetEndTimeManually()
            {
                EndDate = _inputValidator.GetValidDateFromUser();
                EndTime = _inputValidator.GetValidTimeFromUser();
            }

            private void CalculateDurationMinutes()
            {
                if (IsStopWatchEnabled)
                {
                    DurationMinutes = (int)_stopwatch.Elapsed.TotalMinutes;
                }
                else
                {
                    if (!StartTime.HasValue || !EndTime.HasValue || EndTime < StartTime)
                    {
                        throw new InvalidOperationException("Invalid Start or End Time.");
                    }

                    DurationMinutes = (int)((EndTime.Value - StartTime.Value).TotalMinutes);
                }
            }


            public bool CheckBothDurationCalculations()
            {
                if (!IsStopWatchEnabled || !StartTime.HasValue || !EndTime.HasValue)
                {
                    throw new InvalidOperationException("Cannot check durations - either stopwatch is not enabled or manual times are not set.");
                }

                int stopwatchMinutes = (int)_stopwatch.Elapsed.TotalMinutes;
                int manualDurationMinutes = (int)((EndTime.Value - StartTime.Value).TotalMinutes);

                return stopwatchMinutes == manualDurationMinutes;
            }
        }
    }