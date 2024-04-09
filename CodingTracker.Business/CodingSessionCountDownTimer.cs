﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.CodingGoalDTOManagers;

namespace CodingTracker.Business.CodingSessionCountDownTimers
{

    public interface ICodingSessionCountDownTimer
    {
        TimeSpan ConvertGoalHoursAndMinsToTimeSpan(int goalHours, int goalMins);
        TimeSpan setMaxTime();
    }


    public class CodingSessionCountDownTimer : ICodingSessionCountDownTimer
    {
        private readonly IApplicationLogger _appLogger;
        private readonly ICodingGoalDTOManager _codingGoalDTOManager;
        public TimeSpan _maxTime;




        public CodingSessionCountDownTimer(IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
        }

        public TimeSpan setMaxTime()
        {
            var currentGoalDTO = _codingGoalDTOManager.GetCurrentCodingGoalDTO();
            if (currentGoalDTO != null)
            {
                _maxTime = ConvertGoalHoursAndMinsToTimeSpan(currentGoalDTO.GoalHours, currentGoalDTO.GoalMinutes);
            }
            return _maxTime;
        }


        public TimeSpan ConvertGoalHoursAndMinsToTimeSpan(int goalHours, int goalMins)
        {
            TimeSpan goalTimeSpan = new TimeSpan(goalHours, goalMins, 0);
            return goalTimeSpan;
        }
    }
}
