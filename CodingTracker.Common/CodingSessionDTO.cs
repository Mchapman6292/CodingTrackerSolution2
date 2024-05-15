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
        public DateOnly startDate { get; set; } = DateOnly.MinValue;
        public DateTime startTime { get; set; } = DateTime.MinValue;
        public DateOnly endDate { get; set; } = DateOnly.MinValue;
        public DateTime endTime { get; set; } = DateTime.MinValue;
        public double durationSeconds { get; set; } = -1;

        public string durationHHMM { get; set; } = "";

        public string goalHHMM { get; set; } = "";
        public int goalReached { get; set; } = 0;



    }
}