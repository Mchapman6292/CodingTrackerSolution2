using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IDatabaseSessionReads;


namespace CodingTracker.Business.SessionCalculators
{
    public interface ISessionCalculator
    {
        double CalculateLastSevenDaysAvg();
        double CalculateTodayTotal();
        double CalculateTotalAvg();
    }


    public class SessionCalculator : ISessionCalculator
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseSessionRead _databaseSessionRead;
 



        public SessionCalculator(IApplicationLogger appLogger, IDatabaseSessionRead databaseSessionRead)
        { 
            _appLogger = appLogger;
            _databaseSessionRead = databaseSessionRead;
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
                    List<int> last7Days = _databaseSessionRead.ReadSessionDurationMinutes(numberOfDays);

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
                    List<int> todayMins = _databaseSessionRead.ReadSessionDurationMinutes(numberOfDays);

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
                    List<int> totalMins = _databaseSessionRead.ReadSessionDurationMinutes(0, true);

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
    }
}
