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
        public TimeSpan? Duration { get; set; }

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
        }

        private void CalculateDuration()
        {
            if (IsStopWatchEnabled)
            {
                Duration = _stopwatch.Elapsed;
            }
            else
            {
                if (EndTime < StartTime)
                {
                    throw new InvalidOperationException("EndTime cannot be earlier than StartTime.");
                }
                Duration = EndTime - StartTime;
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
    }
}