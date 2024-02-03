using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.IInputvalidator;
using Spectre.Console;
using System.Diagnostics;


// method to record start & end time
// logic to hold recorded times & view them
// user should be able to input start & end times manually 

namespace CodingTracker.CodingSessions
{
    public class CodingSession
    {

        private readonly Stopwatch _stopwatch = new Stopwatch();
        public bool IsStopWatchEnabled = false;

        public CodingSession(IInputValidator validator)
        {
            _validator = validator;

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
            StartDate = _validator.GetValidDateFromUser();
            StartTime = _validator.GetValidTimeFromUser();
        }

        public void SetEndTimeManually()
        {
            EndDate = _validator.GetValidDateFromUser();
            EndTime = _validator.GetValidTimeFromUser();
        }
    }
}