using System;
using System.Diagnostics;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.CodingGoalDTOs;
using System.Data.Entity.Core.Objects;

namespace CodingTracker.Common.CodingGoalDTOManagers
{
    public interface ICodingGoalDTOManager
    {
        CodingGoalDTO CreateCodingGoalDTO(int codingGoalHours, int codingGoalMinutes);
        CodingGoalDTO GetCurrentCodingGoalDTO();
        int ReturnCodingGoalTotalMinutes();
        string FormatCodingGoalHoursMinsToString();

        void UpdateCodingGoalDTO(int codingGoalHours, int codingGoalMinutes);
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
            using (var activity = new Activity(nameof(CreateCodingGoalDTO)).Start())
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug($"Attempting to create a new CodingGoalDTO. TraceID: {activity.TraceId}");

                _currentGoalDTO = new CodingGoalDTO
                {
                    GoalHours = codingGoalHours,
                    GoalMinutes = codingGoalMinutes,
                };

                stopwatch.Stop();

                _appLogger.Info($"New CodingGoalDTO created goalHours: {codingGoalHours}, goalMinutes: {codingGoalMinutes}. Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");
                return _currentGoalDTO;
            }
        }

        public CodingGoalDTO GetCurrentCodingGoalDTO()
        {
            using (var activity = new Activity(nameof(GetCurrentCodingGoalDTO)).Start())
            {
                _appLogger.Debug($"Fetching current CodingGoalDTO. TraceID: {activity.TraceId}");
                return _currentGoalDTO;
            }
        }

        public int ReturnCodingGoalTotalMinutes()
        {
            using (var activity = new Activity(nameof(ReturnCodingGoalTotalMinutes)).Start())
            {
                var stopwatch = Stopwatch.StartNew();

                _appLogger.Debug($"Calculating total minutes from CodingGoalDTO. TraceID: {activity.TraceId}");

                CodingGoalDTO currentCodingGoalDTO = GetCurrentCodingGoalDTO();

                if (currentCodingGoalDTO == null)
                {
                    _appLogger.Error($"Failed to fetch current CodingGoalDTO. TraceID: {activity.TraceId}");
                    return 0;
                }

                int totalGoalMinutes = currentCodingGoalDTO.GoalHours * 60 + currentCodingGoalDTO.GoalMinutes;

                stopwatch.Stop();

                _appLogger.Info($"Calculated total goal minutes: {totalGoalMinutes}. Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");

                return totalGoalMinutes;

            }
        }



        public string FormatCodingGoalHoursMinsToString()
        {
            using (var activity = new Activity(nameof(FormatCodingGoalHoursMinsToString)).Start())
            {
                _appLogger.Debug($"Formatting CodingGoalDTO hours and minutes to string. TraceID: {activity.TraceId}");

                CodingGoalDTO currentGoalDTO = GetCurrentCodingGoalDTO();

                if (currentGoalDTO == null)
                {
                    _appLogger.Error($"Failed to fetch current CodingGoalDTO. TraceID: {activity.TraceId}");
                    return "null value in currentGoalDTO";

                }
                string formattedHours = currentGoalDTO.GoalHours.ToString("D2");
                string formattedMinutes = currentGoalDTO.GoalMinutes.ToString("D2");
                string formattedTime = $"{formattedHours}:{formattedMinutes}";

                _appLogger.Info($"Formatted time: {formattedTime}. TraceID: {activity.TraceId}");

                return formattedTime;

            }
        }

        public void UpdateCodingGoalDTO(int codingGoalHours, int codingGoalMinutes)
        {
            using (var activity = new Activity(nameof(UpdateCodingGoalDTO)).Start())
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Info($"Starting {nameof(UpdateCodingGoalDTO)}. TraceID: {activity.TraceId}");

                if (_currentGoalDTO == null)
                {
                    _appLogger.Info("No current coding goal DTO found. Creating a new one.");
                    _currentGoalDTO = CreateCodingGoalDTO(codingGoalHours, codingGoalMinutes);
                }
                else
                {
                    _currentGoalDTO.GoalHours = codingGoalHours;
                    _currentGoalDTO.GoalMinutes = codingGoalMinutes;
                    _appLogger.Info($"Updated CodingGoalDTO with new hours: {codingGoalHours} and minutes: {codingGoalMinutes}. TraceID: {activity.TraceId}");
                }

                stopwatch.Stop();
                _appLogger.Info($"UpdateCodingGoalDTO completed in {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
            }
        }

    }
}
