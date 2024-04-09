using System;
using System.Diagnostics;
using CodingTracker.Common.CodingGoalDTOs;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.ICodingGoals;
using CodingTracker.Common.CodingGoalDTOManagers;

namespace CodingTracker.Business.CodingGoals
{
    public class CodingGoal : ICodingGoal
    {
        private readonly IApplicationLogger _appLogger;
        private readonly ICodingGoalDTOManager _dtoManager;

        private CodingGoalDTO _currentGoalDTO;
        public int? CodingGoalHours { get; set; }
        public int? CodingGoalMins { get; set; }

        public CodingGoal(IApplicationLogger appLogger, ICodingGoalDTOManager dtoManager)
        {
            _appLogger = appLogger;
            _dtoManager = dtoManager;
        }

        public void SetCodingGoal(int goalHours)
        {
            var methodStopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(SetCodingGoal)).Start())

            {
                _appLogger.Debug($"Starting {nameof(SetCodingGoal)}. TraceID: {activity.TraceId}, GoalHours: {goalHours}");
                try
                {
                    _dtoManager.GetCurrentCodingGoalDTO();
                    CodingGoalHours = _currentGoalDTO.GoalHours;
                    CodingGoalMins = _currentGoalDTO.GoalMinutes;
                    methodStopwatch.Stop();

                    _appLogger.Debug($"Coding goal set. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}, CodingGoalHours: {CodingGoalHours}.");
                }
                catch (Exception ex)
                {
                    methodStopwatch.Stop();
                    _appLogger.Error($"An error occurred during {nameof(SetCodingGoal)}. Execution Time: {methodStopwatch.ElapsedMilliseconds}ms. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }
    }
}
