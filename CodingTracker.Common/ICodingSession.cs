﻿using CodingTracker.Common.CodingSessionDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.ICodingSessions
{
    public interface ICodingSession
    {
        CodingSessionDTO GetCurrentSessionDTO();
        void StartSession(int userId);
        void EndSession();
        void SaveCurrentCodingSession();
        bool CheckIfCodingSessionActive();
        void SetStartTimeManually();
        void SetEndTimeManually();
        void CalculateDurationMinutes();
        bool CheckBothDurationCalculations();
        void CalculateTimeToGoal();





    }
}
