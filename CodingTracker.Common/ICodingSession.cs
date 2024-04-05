using CodingTracker.Common.CodingSessionDTOs;
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
        void StartCodingSessionTimer();

        void EndCodingSessionTimer();
        void StartSession();
        void EndSession();
        void SetSessionEndTimeAndDate();
        bool CheckIfCodingSessionActive();
        void CalculateDurationMinutes();
        bool CheckBothDurationCalculations();
        void CalculateTimeToGoal();

        List<DateTime> GetDatesPrevious28days();





    }
}
