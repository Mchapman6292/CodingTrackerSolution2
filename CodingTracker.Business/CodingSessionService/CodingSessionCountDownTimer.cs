using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IApplicationLoggers;

namespace CodingTracker.Business.CodingSessionService
{

    public interface ICodingSessionCountDownTimer
    {
        TimeSpan ConvertGoalHoursAndMinsToTimeSpan(int goalHours, int goalMins);
        TimeSpan setMaxTime();
    }


    public class CodingSessionCountDownTimer : ICodingSessionCountDownTimer
    {
        private readonly IApplicationLogger _appLogger;
        public TimeSpan _maxTime;




        public CodingSessionCountDownTimer(IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
        }

        public TimeSpan setMaxTime()
        {
            throw new NotImplementedException();
        }


        public TimeSpan ConvertGoalHoursAndMinsToTimeSpan(int goalHours, int goalMins)
        {
            TimeSpan goalTimeSpan = new TimeSpan(goalHours, goalMins, 0);
            return goalTimeSpan;
        }
    }
}
