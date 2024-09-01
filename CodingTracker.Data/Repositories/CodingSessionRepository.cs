using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CodingTracker.Common.CodingSessions;
using CodingTracker.Common.Interfaces.ICodingSessionRepository;
using CodingTracker.Data.EntityContexts;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.DataInterfaces.IEntityContexts;
using System.Diagnostics;
using System.Security.Cryptography;


// Migrate changes to codingSession(removal of goal) once build errors fixed.

namespace CodingTracker.Common.DataInterfaces.CodingSessionRepository
{
    public class CodingSessionRepository : ICodingSessionRepository
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IEntityContext _context;
        public CodingSessionRepository(IEntityContext context, IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
            _context = context;
        }


        public virtual async Task<CodingSession> GetByIdAsync(Activity activity, int id)
        {
            throw new NotImplementedException();
        }


        public async Task<bool> AddCodingSession(Activity activity, CodingSession session)
        {
            _appLogger.Info($"Starting {nameof(AddCodingSession)} for session with ID {session.SessionId}. TraceId: {activity.TraceId}.");
            try
            {
                _context.CodingSessions.Add(session);
                await _context.SaveChangesAsync();

                _appLogger.Info($"Successfully added coding session with ID {session.SessionId}. TraceId: {activity.TraceId}.");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error adding coding session with ID {session.SessionId} for {nameof(AddCodingSession)}, TraceId: {activity.TraceId}.", ex);
                return false;
            }
        }

        public async Task<IEnumerable<CodingSession>> GetAllCodingSessions(Activity activity)
        {
            _appLogger.Info($"Starting {nameof(GetAllCodingSessions)}. TraceId: {activity.TraceId}");
            try
            {
                var result = await _context.CodingSessions
                    .OrderByDescending(cs => cs.StartDate)
                    .ToListAsync();

                if (result.Count > 0)
                {
                    _appLogger.Info($"Retrieved {result.Count} sessions for {nameof(GetAllCodingSessions)}. TraceId: {activity.TraceId}.");
                }
                else
                {
                    _appLogger.Info($"No sessions found for {nameof(GetAllCodingSessions)}. TraceId: {activity.TraceId}.");
                }
                return result;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error retrieving sessions for {nameof(GetAllCodingSessions)}, TraceId: {activity.TraceId}.", ex);
                return Enumerable.Empty<CodingSession>();
            }
        }




        public async Task<(bool success, List<int> failedIds)> DeleteSessionsById(Activity activity, List<int> sessionIds)
        {
            _appLogger.Info($"Starting {nameof(DeleteSessionsById)}, for {sessionIds.Count()} TraceId: {activity.TraceId}.");

            List<int> failedIds = new List<int>();
            bool overallSuccess = true;

            try
            {
                var entitiesToDelete = await _context.CodingSessions
                         .Where(s => sessionIds.Contains(s.SessionId))
                         .ToListAsync();

                foreach (var entity in entitiesToDelete)
                {
                    _context.CodingSessions.Remove(entity);
                }

                failedIds.AddRange(sessionIds.Except(entitiesToDelete.Select(e => e.SessionId)));  // Adds SessionIds that were not found in the database

                if (entitiesToDelete.Any())
                {
                    await _context.SaveChangesAsync();
                    _appLogger.Info($"Successfully deleted {entitiesToDelete.Count} sessions. TraceId: {activity.TraceId}");
                }


                if (failedIds.Any())
                {
                    overallSuccess = false;
                    _appLogger.Warning($"Failed to delete {failedIds.Count} sessions. Session IDs: {string.Join(", ", failedIds)}. TraceId: {activity.TraceId}");
                }
            }
            catch (Exception ex)
            {
                overallSuccess = false;
                _appLogger.Error($"Error occurred while deleting sessions. TraceId: {activity.TraceId}.", ex);
                failedIds = sessionIds;
            }

            return (overallSuccess, failedIds);
        }

        public async Task<IEnumerable<CodingSession>> GetRecentSessions(Activity activity, int numberOfSessions)
        {
            _appLogger.Info($"Starting {nameof(GetRecentSessions)} TraceId: {activity.TraceId}");

            try
            {
                var sessions = await _context.CodingSessions
                            .OrderByDescending(s => s.StartDate)
                            .Take(numberOfSessions)
                            .ToListAsync();

                if (sessions.Any())
                {
                    _appLogger.Info($"Retrieved {sessions.Count} sessions for {nameof(GetRecentSessions)}. TraceId: {activity.TraceId}.");
                }
                else
                {
                    _appLogger.Info($"No sessions found for for {nameof(GetRecentSessions)} TraceId: {activity.TraceId}.");
                }
                return sessions;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error retrieving sessions for {nameof(GetCodingSessionByDateOnly)}, TraceId: {activity.TraceId}.", ex);
                return Enumerable.Empty<CodingSession>();
            }
        }

        public async Task<IEnumerable<CodingSession>> GetCodingSessionByDateOnly(Activity activity, DateOnly startDate, DateOnly endDate) 
        {
            _appLogger.Info($"Starting {nameof(GetCodingSessionByDateOnly)} TraceId: {activity.TraceId}");

            try
            {
                var result = await _context.CodingSessions
                    .Where(cs => cs.StartDate <= endDate && cs.EndDate >= startDate)
                    .OrderByDescending(cs => cs.StartDate)
                    .ToListAsync();

                if (result.Count > 0)
                {
                    _appLogger.Info($"Retrieved {result.Count} sessions for {nameof(GetCodingSessionByDateOnly)}. TraceId: {activity.TraceId}.");
                }
                else
                {
                    _appLogger.Info($"No sessions found for for {nameof(GetCodingSessionByDateOnly)} TraceId: {activity.TraceId}.");
                }
                return result;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error retrieving sessions for {nameof(GetCodingSessionByDateOnly)}, TraceId: {activity.TraceId}.", ex);
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








