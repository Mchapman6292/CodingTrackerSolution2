using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.CodingSessions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.ICodingSessionManagers
{
    public interface ICodingSessionManager
    {
        CodingSession CreateNewCodingSession(int userId, Activity activity);
        void StartCodingSession(CodingSession codingSession, Activity activity, int userId);
        void EndCodingSession(Activity activity);
        string ConvertDurationSecondsToStringHHMM(Activity activity, int durationSeconds);
        void UpdateCodingSessionEndTimes(Activity activity);
        void SetCurrentCodingSession(CodingSession codingSession, Activity activity);
        int CalculateDurationSeconds(Activity activity, DateTime? startDate, DateTime? endDate);


    }
}