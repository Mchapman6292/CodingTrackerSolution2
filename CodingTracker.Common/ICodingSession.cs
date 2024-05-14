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
        int CurrentSessionId { get; set; }
        int CurrentUserId { get; set; }
        DateTime CurrentStartDate { get; set; }
        DateTime CurrentStartTime { get; set; }
        DateTime CurrentEndDate { get; set; }
        DateTime CurrentEndTime { get; set; }
        double CurrentDurationSeconds { get; set; }
        string CurrentDurationHHMM { get; set; }
        string CurrentGoalHHMM { get; set; }
        int CurrentGoalReached { get; set; }
    


    void StartSession();
    void EndSession();
     bool CheckIfCodingSessionActive();

    List<DateTime> GetDatesPrevious28days();





    }
}
