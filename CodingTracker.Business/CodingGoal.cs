using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.ICodingGoals;
using CodingTracker.Common.ICodingSessions;

namespace CodingTracker.Business.CodingGoals
{
    public class CodingGoal : ICodingGoal
    {
        private readonly IApplicationLogger _appLogger;
        public int? CodingGoalHours { get; set; }
        public int? TimeToGoalMinutes { get; set; }


        public CodingGoal(IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
        }


        public void SetCodingGoal(int goalHours)
        {
            using (var activity = new Activity(nameof(SetCodingGoal)).Start())
            {
                _appLogger.Info($"Starting {nameof(SetCodingGoal)}. TraceID: {activity.TraceId}, GoalHours: {goalHours}");
                try
                {
                    CodingGoalHours = goalHours;
                    TimeToGoalMinutes = CodingGoalHours * 60;

                    _appLogger.Info($"Coding goal set. TraceID: {activity.TraceId}, CodingGoalHours: {CodingGoalHours}, TimeToGoalMinutes: {TimeToGoalMinutes}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(SetCodingGoal)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }

        public string FormatTimeToGoalToHHMM(int? timeToGoal)
        {
            using (var activity = new Activity(nameof(FormatTimeToGoalToHHMM)).Start())
            {
                _appLogger.Info($"Formatting time to goal in HHMM. TraceID: {activity.TraceId}, TimeToGoal: {timeToGoal}");
                try
                {
                    if (!timeToGoal.HasValue)
                    {
                        throw new ArgumentException("Minutes value is required.");
                    }

                    int totalMinutes = timeToGoal.Value;
                    int hours = totalMinutes / 60;
                    int remainingMinutes = totalMinutes % 60;
                    string formattedTime = $"{hours:D2}:{remainingMinutes:D2}";

                    _appLogger.Info($"Time to goal formatted. TraceID: {activity.TraceId}, FormattedTime: {formattedTime}");
                    return formattedTime;
                }
                catch (Exception ex)
                { 
                    _appLogger.Error($"An error occurred during {nameof(FormatTimeToGoalToHHMM)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    return null;
                }
            }
        }
    }
}
