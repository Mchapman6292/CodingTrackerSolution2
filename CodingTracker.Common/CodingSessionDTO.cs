using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.CodingSessionDTOs
{
    public class CodingSessionDTO
    {
        public int sessionId { get; set; } = 0;// Default value indicating not set. 
        public int userId { get; set; } = 0;
        public DateTime? startDate { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? endDate { get; set; }
        public DateTime? endTime { get; set; }
        public double? durationSeconds { get; set; }

        public string? durationHHMM { get; set; }

        public string? goalHHMM { get; set; }
        public int goalReached { get; set; } = 0;



    }
}