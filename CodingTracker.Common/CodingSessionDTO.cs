using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.CodingSessionDTOs
{
    public class CodingSessionDTO
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? Duration { get; set; }

        public int? CodingGoal { get; set; }

        public int ProgressHours { get; set; }
        
        public int ProgressMinutes {  get; set; }

        public string? SessionNotes { get; set; }
    }
}