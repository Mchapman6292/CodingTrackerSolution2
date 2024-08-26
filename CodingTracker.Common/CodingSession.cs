using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.UserCredentials;

namespace CodingTracker.Common.CodingSessions
{
    public class CodingSession
    {
        [Key]
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateOnly? EndDate { get; set; }
        public DateTime? EndTime { get; set; }
        public double? DurationSeconds { get; set; }
        public string DurationHHMM { get; set; } = string.Empty;
        public string GoalHHMM { get; set; } = string.Empty;
        public int? GoalReached { get; set; }

 

        public UserCredential User { get; set; }

        public CodingSession()
        {
            SessionId = AssignCodingSessionId();
        }



        public CodingSession CreateNewCodingSession(int sessionId, int userID)
        { }


        public int AssignCodingSessionId()
        {
            throw new NotImplementedException();
        }
    }
}
