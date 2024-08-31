using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.CodingSessionDTOManagers;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.Interfaces.ICodingSessionRepository;


namespace CodingTracker.Business.SessionCalculators
{
    public interface ISessionCalculator
    {
        Task<double> CalculateLastSevenDaysAvgInSeconds();
        Task<int> CalculateTodayTotal();
        Task<double> CalculateTotalAvg();
        double CalculateDurationSeconds();
    }
    

    public class SessionCalculator : ISessionCalculator
    {
        private readonly IApplicationLogger _appLogger;
        private readonly ICodingSessionRepository _codingSessionRepository;

        public SessionCalculator(IApplicationLogger appLogger, ICodingSessionRepository codingSessionRepository)
        {
            _appLogger = appLogger;
            _codingSessionRepository = codingSessionRepository;
        }


        public async Task<double> CalculateLastSevenDaysAvgInSeconds()
        {
            using (var activity = new Activity(nameof(CalculateLastSevenDaysAvgInSeconds)).Start())
            {
                _appLogger.Debug($"Starting {nameof(CalculateLastSevenDaysAvgInSeconds)}. TraceID: {activity.TraceId}");
                try
                {
                    DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
                    DateOnly dateSevenDaysAgo = currentDate.AddDays(-6); // -6 to include today

                    var sessions = await _codingSessionRepository.GetCodingSessionByDateOnly(activity, dateSevenDaysAgo, currentDate);

                    if (!sessions.Any())
                    {
                        _appLogger.Info($"No sessions found in the last 7 days. TraceID: {activity.TraceId}");
                        return 0;
                    }

                    var sessionsByDate = sessions
                        .GroupBy(s => s.StartDate)
                        .Select(g => new { Date = g.Key, TotalSeconds = g.Sum(s => s.DurationSeconds ?? 0) })
                        .ToList();

                    double totalSeconds = sessionsByDate.Sum(s => s.TotalSeconds);
                    int daysWithSessions = sessionsByDate.Count;

                    double averageSeconds = totalSeconds / 7; // Still divide by 7 for a true 7-day average

                    _appLogger.Info($"Calculated last 7 days average successfully. Average: {averageSeconds:F2} seconds per day. Days with sessions: {daysWithSessions}. TraceID: {activity.TraceId}");
                    return averageSeconds;
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"Failed to calculate last 7 days average. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    throw;
                }
            }
        }
        public async Task<int> CalculateTodayTotal()
        {
            using (var activity = new Activity(nameof(CalculateTodayTotal)).Start())
            {
                _appLogger.Debug($"Starting {nameof(CalculateTodayTotal)}. TraceID: {activity.TraceId}");
                try
                {
                    DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
                    var sessions = await _codingSessionRepository.GetCodingSessionByDateOnly(activity, today, today);

                    if (!sessions.Any())
                    {
                        _appLogger.Info($"No sessions found for today. TraceID: {activity.TraceId}");
                        return 0;
                    }

                    int totalDurationSeconds = sessions.Sum(s => s.DurationSeconds ?? 0);

                    _appLogger.Info($"Calculated today's total successfully. Total: {totalDurationSeconds} seconds. TraceID: {activity.TraceId}");
                    return totalDurationSeconds;
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"Failed to calculate today's total. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    return 0;
                }
            }
        }
        
        
            
        public async Task<double> CalculateTotalAvg()
        {
            using (var activity = new Activity(nameof(CalculateTotalAvg)).Start())
            {
                _appLogger.Debug($"Starting {nameof(CalculateTotalAvg)}. TraceID: {activity.TraceId}");
                try
                {
                    var allSessions = await _codingSessionRepository.GetAllCodingSessions(activity);

                    if (!allSessions.Any())
                    {
                        _appLogger.Info($"No sessions found for average calculation. TraceID: {activity.TraceId}");
                        return 0;
                    }

                    double totalSeconds = allSessions.Sum(s => s.DurationSeconds ?? 0);
                    int sessionCount = allSessions.Count();
                    double averageSeconds = totalSeconds / sessionCount;

                    _appLogger.Info($"Calculated total average successfully. Average: {averageSeconds:F2} seconds per session. Total sessions: {sessionCount}. TraceID: {activity.TraceId}");
                    return averageSeconds;
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"Failed to calculate total average. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    return 0;
                }
            }
        }




        public double CalculateDurationSeconds()
        {
            throw new NotImplementedException();
        }
    }
}
