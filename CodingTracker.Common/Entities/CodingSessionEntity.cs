using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.Entities.CodingSessionEntities
{
    public class CodingSessionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SessionId { get; set; } // Default value indicating not set. 
        public DateTime? StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public double? DurationSeconds { get; set; }

        public string? DurationHHMM { get; set; }

        public string? GoalHHMM { get; set; }
        public int? GoalReached { get; set; }
    }
}
