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
using System.Diagnostics;


// Migrate changes to codingSession(removal of goal) once build errors fixed.

namespace CodingTracker.Common.DataInterfaces.CodingSessionRepository
{
    public class CodingSessionRepository : GenericRepository<CodingSession>, ICodingSessionRepository
    {
        private readonly IApplicationLogger _appLogger;
        public CodingSessionRepository(EntityContext context, IApplicationLogger appLogger) : base(context)
        {
            _appLogger = appLogger;
        }


        public async Task<bool> AddCodingSession(CodingSession session, string traceId)
        {
            _appLogger.Info($"Starting {nameof(AddCodingSession)} for session with ID {session.SessionId}. TraceId: {traceId}.");
            try
            {
                _context.CodingSessions.Add(session);
                await _context.SaveChangesAsync();

                _appLogger.Info($"Successfully added coding session with ID {session.SessionId}. TraceId: {traceId}.");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error adding coding session with ID {session.SessionId} for {nameof(AddCodingSession)}, TraceId: {traceId}.", ex);
                return false;
            }
        }


        public async Task<IEnumerable<CodingSession>> GetSessionsByDateRange(DateOnly startDate, DateOnly endDate, string traceId)
        {
            _appLogger.Info($"Starting {nameof(GetSessionsByDateRange)} for sessions from {startDate} to {endDate}. TraceId: {traceId}.");
            try
            {
                var result = await _dbSet
                   .Where(cs => cs.StartDate >= startDate && cs.EndDate <= endDate)
                   .OrderByDescending(cs => cs.StartDate)
                   .ThenBy(cs => cs.StartTime)
                   .ToListAsync();


                if (result.Count > 0)
                {
                    _appLogger.Info($"Retrieved {result.Count} sessions for {nameof(GetSessionsByDateRange)}. TraceId: {traceId}.");
                }
                else
                {
                    _appLogger.Info($"No sessions found for for {nameof(GetSessionsByDateRange)} TraceId: {traceId}.");
                }
                return result;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error retrieving sessions for {nameof(GetSessionsByDateRange)}, TraceId: {traceId}.", ex);
                return Enumerable.Empty<CodingSession>();
            }
        }

        public async Task<(bool success, List<int> failedIds)> DeleteSessionsById(List<int> sessionIds, string traceId)
        {
            _appLogger.Info($"Starting {nameof(DeleteSessionsById)}, for {sessionIds.Count()} TraceId: {traceId}.");

            List<int> failedIds = new List<int>();
            bool overallSuccess = true;

            try
            {
                var entitiesToDelete = await _dbSet
                         .Where(s => sessionIds.Contains(s.SessionId))
                         .ToListAsync();

                foreach (var entity in entitiesToDelete)
                {
                    _dbSet.Remove(entity);
                }

                failedIds.AddRange(sessionIds.Except(entitiesToDelete.Select(e => e.SessionId)));  // Adds SessionIds that were not found in the database

                if (entitiesToDelete.Any())
                {
                    await _context.SaveChangesAsync();
                    _appLogger.Info($"Successfully deleted {entitiesToDelete.Count} sessions. TraceId: {traceId}");
                }


                if (failedIds.Any())
                {
                    overallSuccess = false;
                    _appLogger.Warning($"Failed to delete {failedIds.Count} sessions. Session IDs: {string.Join(", ", failedIds)}. TraceId: {traceId}");
                }
            }
            catch (Exception ex)
            {
                overallSuccess = false;
                _appLogger.Error($"Error occurred while deleting sessions. TraceId: {traceId}.", ex);
                failedIds = sessionIds;
            }

            return (overallSuccess, failedIds);
        }



        public async Task<IEnumerable<CodingSession>> GetCodingSessionByDateOnly(DateOnly startDate, DateOnly endDate, string traceId)
        {
            _appLogger.Info($"Starting {nameof(GetCodingSessionByDateOnly)} TraceId: {traceId}");

            try
            {
                var result = await _dbSet
                    .Where(cs => cs.StartDate >= startDate && cs.EndDate <= endDate)
                    .OrderByDescending(cs => cs.StartDate)
                    .ToListAsync();

                if (result.Count > 0)
                {
                    _appLogger.Info($"Retrieved {result.Count} sessions for {nameof(GetCodingSessionByDateOnly)}. TraceId: {traceId}.");
                }
                else
                {
                    _appLogger.Info($"No sessions found for for {nameof(GetCodingSessionByDateOnly)} TraceId: {traceId}.");
                }
                return result;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error retrieving sessions for {nameof(GetCodingSessionByDateOnly)}, TraceId: {traceId}.", ex);
                return Enumerable.Empty<CodingSession>();
            }
        }


        public async Task<bool> SaveCodingSessionChanges(Activity activity)
        {
            try
            {
                await _context.SaveChangesAsync();
                _appLogger.Info($" SaveCodingSessionChanges successful TraceId: {activity.TraceId}, ParentId: {activity.ParentId}.");
                return true;
            }
            catch (DbUpdateException ex)
            {
                _appLogger.Error($"Database error during {nameof(SaveCodingSessionChanges)}. TraceId: {activity.TraceId}, ParentId: {activity.ParentId}", ex);
                return false;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Unexpected error during {nameof(SaveCodingSessionChanges)}. TraceId: {activity.TraceId}, ParentId: {activity.ParentId}", ex);
                throw;

            }
        }


    }
}








