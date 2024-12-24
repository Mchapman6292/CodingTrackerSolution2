using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.UserCredentials;
using CodingTracker.Common.IdGenerators;
using System.Diagnostics;
using CodingTracker.Common.IApplicationLoggers;


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
        public int? DurationSeconds { get; set; }
        public string DurationHHMM { get; set; } = string.Empty;










    }
}
