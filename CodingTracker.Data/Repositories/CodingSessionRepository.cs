using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CodingTracker.Data.Interfaces.ICodingSessionRepository;
using CodingTracker.Common.CodingSessions;
using CodingTracker.Data.Repositories.GenericRepository;
using CodingTracker.Data.EntityContexts;
using CodingTracker.Common.IApplicationLoggers;

namespace CodingTracker.Data.Repositories.CodingSessionRepository
{
    public class CodingSessionRepository : GenericRepository<CodingSession>, ICodingSessionRepository
    {
        private readonly IApplicationLogger _appLogger;
        public CodingSessionRepository(EntityContext context, IApplicationLogger appLogger) : base(context)
        {
            _appLogger = appLogger;
        }


        public async Task<IEnumerable<CodingSession>> GetSessionsByDateRange(int userId, DateOnly startDate, DateOnly endDate, string traceId)
        {
            _appLogger.Info($"Starting {nameof(GetSessionsByDateRange)} for sessions from {startDate} to {endDate}. TraceId: {traceId}.");
            try
            {
                var result = await _dbSet
                   .Where(cs => cs.UserId == userId && cs.StartDate >= startDate && cs.EndDate <= endDate)
                   .OrderByDescending(cs => cs.StartDate)
                   .OrderByDescending(cs => cs.StartDate)
                   .ThenBy(cs => cs.StartTime)
                   .ToListAsync();

                _appLogger.Info($"Retrieved {result.Count} sessions. TraceId: {traceId}.");

                return result;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error retrieving sessions for{nameof(GetSessionsByDateRange)}, TraceId: {traceId}.", ex);

                return Enumerable.Empty<CodingSession>();
            }
        }

        public async Task<IEnumerable<CodingSession>> GetDurationSecondsLast28Days(DateOnly startDate, DateOnly endDate, string traceId)
        {
            _appLogger.Info($"Starting {nameof(GetDurationSecondsLast28Days)} for sessions from {startDate} to {endDate}. TraceId: {traceId}.");
            try
            {
                var result = await _dbSet
                    .Where(cs => cs.StartDate >= startDate && cs.EndDate <= endDate)
                    .OrderByDescending(cs => cs.StartDate)
                    .ThenByDescending(cs => cs.StartTime)
                    .ToListAsync();

                _appLogger.Info($"Retrieved {result.Count} sessions. TraceId: {traceId}.");

                return result;

            }
            catch(Exception ex) 
            {
                _appLogger.Error($"Error retrieving sessions for {nameof(GetDurationSecondsLast28Days)}, TraceId: {traceId}.", ex);

                return Enumerable.Empty<CodingSession> ();
            }



            }
        }





            }

    }
}
