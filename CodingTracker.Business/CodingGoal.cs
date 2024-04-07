using System;
using System.Diagnostics;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.ICodingGoals;

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
            var methodStopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(SetCodingGoal)).Start())
            {
                _appLogger.Info($"Starting {nameof(SetCodingGoal)}. TraceID: {activity.TraceId}, GoalHours: {goalHours}");
                try
                {
                    CodingGoalHours = goalHours;
                    TimeToGoalMinutes = CodingGoalHours * 60;
                    methodStopwatch.Stop();

                    _appLogger.Info($"Coding goal set. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}, CodingGoalHours: {CodingGoalHours}.");
                }
                catch (Exception ex)
                {
                    methodStopwatch.Stop();
                    _appLogger.Error($"An error occurred during {nameof(SetCodingGoal)}. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }

        public string FormatTimeToGoalToHHMM(int? timeToGoal)
        {
            var methodStopwatch = Stopwatch.StartNew();
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
                    methodStopwatch.Stop();

                    _appLogger.Info($"Time to goal formatted. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}, FormattedTime: {formattedTime}");
                    return formattedTime;
                }
                catch (Exception ex)
                {
                    methodStopwatch.Stop();
                    _appLogger.Error($"An error occurred during {nameof(FormatTimeToGoalToHHMM)}. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    return null;
                }
            }
        }
    }
}
