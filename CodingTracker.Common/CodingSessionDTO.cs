using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.CodingSessionDTOs
{
    public class CodingSessionDTO
    {
        public int SessionId { get; set; } = 0; // Default value indicating not set. 
        public int UserId { get; set; } = 0;
        public DateTime? StartDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EndTime { get; set; }
        public double? DurationSeconds { get; set; }

        public string? DurationHHMM { get; set; }

        public string? GoalHHMM { get; set; }
        public int? GoalReached { get; set; }



    }
}