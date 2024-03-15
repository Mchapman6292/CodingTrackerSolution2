using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.ICodingGoals
{
    public interface ICodingGoal
    {
        void SetCodingGoal(int goalHours);
        string FormatTimeToGoalToHHMM(int? timeToGoal);
        void CalculateTimeToGoal();

    }
}
