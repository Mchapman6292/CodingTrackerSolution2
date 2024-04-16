using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IDatabaseSessionReads;
using CodingTracker.Common.CodingSessionDTOManagers;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.CodingGoalDTOs;
using CodingTracker.Common.CodingGoalDTOManagers;

namespace CodingTracker.Business.SessionCalculators
{
    public interface ISessionCalculator
    {
        double CalculateLastSevenDaysAvg();
        double CalculateTodayTotal();
        double CalculateTotalAvg();
        double CalculateDurationSeconds();
    }


    public class SessionCalculator : ISessionCalculator
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseSessionRead _databaseSessionRead;
        private readonly ICodingSessionDTOManager _codingSessionDTOManager;
        private readonly ICodingGoalDTOManager _codingGoalDTOManager;

        public SessionCalculator(IApplicationLogger appLogger, IDatabaseSessionRead databaseSessionRead, ICodingSessionDTOManager codingSessionDTOManager, ICodingGoalDTOManager codingGoalDTOManager)
        {
            _appLogger = appLogger;
            _databaseSessionRead = databaseSessionRead;
            _codingSessionDTOManager = codingSessionDTOManager;
            _codingGoalDTOManager = codingGoalDTOManager;
        }


        public double CalculateLastSevenDaysAvg()
        {
            using (var activity = new Activity(nameof(CalculateLastSevenDaysAvg)).Start())
            {
                _appLogger.Debug($"Starting {nameof(CalculateLastSevenDaysAvg)}. TraceID: {activity.TraceId}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    int numberOfDays = 7;
                    List<double> last7Days = _databaseSessionRead.ReadSessionDurationSeconds(numberOfDays);

                    double averageMinutes = 0;
                    if (last7Days.Count > 0)
                    {
                        double totalMinutes = last7Days.Sum();
                        averageMinutes = totalMinutes / numberOfDays;
                    }

                    stopwatch.Stop();
                    _appLogger.Info($"Calculated last 7 days average successfully. Average: {averageMinutes}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");

                    return averageMinutes;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to calculate last 7 days average. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    throw;
                }
            }
        }

        public double CalculateTodayTotal()
        {
            using (var activity = new Activity(nameof(CalculateTodayTotal)).Start())
            {
                _appLogger.Debug($"Starting {nameof(CalculateTodayTotal)}. TraceID: {activity.TraceId}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    int numberOfDays = 1;
                    List<double> todayMins = _databaseSessionRead.ReadSessionDurationSeconds(numberOfDays);

                    double totalMinutes = 0;
                    if (todayMins.Count > 0)
                    {
                        totalMinutes = todayMins.Sum();
                    }

                    stopwatch.Stop();
                    _appLogger.Info($"Calculated today's total minutes successfully. Total: {totalMinutes}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");

                    return totalMinutes;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to calculate today's total minutes. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    throw;
                }
            }
        }

        public double CalculateTotalAvg()
        {
            using (var activity = new Activity(nameof(CalculateTotalAvg)).Start())
            {
                _appLogger.Debug($"Starting {nameof(CalculateTotalAvg)}. TraceID: {activity.TraceId}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    List<double> totalMins = _databaseSessionRead.ReadSessionDurationSeconds(0, true);

                    double averageMinutes = 0;
                    if (totalMins.Count > 0)
                    {
                        double totalMinutes = totalMins.Sum();
                        averageMinutes = totalMinutes / totalMins.Count;
                    }

                    stopwatch.Stop();
                    _appLogger.Info($"Calculated total average successfully. Average: {averageMinutes}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");

                    return averageMinutes;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to calculate total average. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    throw;
                }
            }
        }

        public int CalculateGoalSeconds()
        {
            using (var activity = new Activity(nameof(CalculateGoalSeconds)).Start())
            {
                _appLogger.Debug($"Starting {nameof(CalculateGoalSeconds)}. TraceID: {activity.TraceId}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                int totalSeconds = 0;
                try
                {
                    CodingGoalDTO currentGoalDTO = _codingGoalDTOManager.GetCurrentCodingGoalDTO();
                    if (currentGoalDTO == null)
                    {
                        _appLogger.Error($"No current CodingGoalDTO found. TraceID: {activity.TraceId}");
                        return 0;
                    }

                    totalSeconds = (currentGoalDTO.GoalHours * 3600) + (currentGoalDTO.GoalMinutes * 60);

                    _appLogger.Info($"Calculated goal seconds successfully. TotalSeconds: {totalSeconds}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to calculate goal seconds. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    throw;
                }

                stopwatch.Stop();
                return totalSeconds;
            }
        }


        public double CalculateDurationSeconds()
        {
            CodingSessionDTO currentSessionDTO = _codingSessionDTOManager.GetCurrentSessionDTO();
            double durationSeconds = 0;
            using (var activity = new Activity(nameof(CalculateDurationSeconds)).Start())
            {
                _appLogger.Info($"Calculating duration seconds. TraceID: {activity.TraceId}");

                try
                {
                    if (!currentSessionDTO.StartTime.HasValue || !currentSessionDTO.EndTime.HasValue)
                    {
                        _appLogger.Error("start Time or End Time is not set.");
                    }

                    TimeSpan duration = currentSessionDTO.EndTime.Value - currentSessionDTO.StartTime.Value;
                    durationSeconds = (double)duration.TotalSeconds;
                    

                    _appLogger.Info($"Duration seconds calculated: {durationSeconds}. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(CalculateDurationSeconds)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
            return durationSeconds;
        }
    }
}
