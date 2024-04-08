using System;
using System.Diagnostics;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.CodingGoalDTOs;

namespace CodingTracker.Common.CodingGoalDTOManagers
{
    public interface ICodingGoalDTOManager
    {
        CodingGoalDTO CreateCodingGoalDTO(int codingGoalHours, int codingGoalMinutes);
        CodingGoalDTO GetCurrentCodingGoalDTO();
    }

    public class CodingGoalDTOManager : ICodingGoalDTOManager
    {
        private readonly IApplicationLogger _appLogger;
        private CodingGoalDTO _currentGoalDTO;
        private int _codingGoalTotalMinutes;

        public CodingGoalDTOManager(IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
        }

        public CodingGoalDTO CreateCodingGoalDTO(int codingGoalHours, int codingGoalMinutes)
        {
            var stopwatch = Stopwatch.StartNew();
            _appLogger.Debug($"Starting {nameof(CreateCodingGoalDTO)}. TraceID: {Activity.Current?.TraceId}");

            _currentGoalDTO = new CodingGoalDTO
            {
                GoalHours = codingGoalHours,
                GoalMinutes = codingGoalMinutes,
            };

            stopwatch.Stop();
            _appLogger.Info($"New CodingGoalDTO created goalHours: {codingGoalHours}, goalMinutes: {codingGoalMinutes}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {Activity.Current?.TraceId}");

            return _currentGoalDTO;
        }

        public CodingGoalDTO GetCurrentCodingGoalDTO()
        {
            return _currentGoalDTO;
        }

        public int ReturnCodingGoalTotalMinutes()
        {
            var stopwatch = Stopwatch.StartNew();
            _appLogger.Debug($"Starting {nameof(ReturnCodingGoalTotalMinutes)}. TraceID: {Activity.Current?.TraceId}");

            int totalGoalMinutes = _currentGoalDTO.GoalHours * 60 + _currentGoalDTO.GoalMinutes;

            stopwatch.Stop();
            _appLogger.Info($"Calculated total goal minutes: {totalGoalMinutes}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {Activity.Current?.TraceId}");

            return totalGoalMinutes;
        }

    }
}
