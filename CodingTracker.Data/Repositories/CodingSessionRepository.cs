using Microsoft.EntityFrameworkCore;
using CodingTracker.Common.DataInterfaces.ICodingSessionRepositories;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.DataInterfaces.ICodingTrackerDbContexts;
using CodingTracker.Common.Entities.CodingSessionEntities;


namespace CodingTracker.Data.Repositories.CodingSessionRepositories
{
    public class CodingSessionRepository : ICodingSessionRepository
    {

        private readonly IApplicationLogger _appLogger;
        private readonly ICodingTrackerDbContext _dbContext;


        public CodingSessionRepository(IApplicationLogger appLogger, ICodingTrackerDbContext context) 
        {
            _appLogger = appLogger;
            _dbContext = context;
        }


        public async Task<List<CodingSessionEntity>> GetSessionsbyIDAsync(List<int> sessionIds)
        {
            // Creating a list of Coding sessions would load all sessions into memory.
            // Using .Where = SELECT * FROM CodingSessions WHERE SessionId IN (1,2,3)

            return await _dbContext.CodingSessions
                    .Where(s => sessionIds.Contains(s.SessionId))
                    .ToListAsync();
        }


        public async Task<int> DeleteSessionsByIdAsync(IReadOnlyCollection<int> sessionIds)
        {
            return await _dbContext.CodingSessions
                .Where(s => sessionIds.Contains(s.SessionId))
                .ExecuteDeleteAsync();
        }


        public async Task<List<CodingSessionEntity>> GetRecentSessionsAsync(int numberOfSessions)
        {
            return await _dbContext.CodingSessions
                    .OrderByDescending(s => s.StartDate)
                    .Take(numberOfSessions)
                    .ToListAsync();
        }

        public async Task<List<CodingSessionEntity>> GetSessionsForLastDaysAsync(int numberOfDays)
        {
            DateTime targetDate = DateTime.UtcNow.AddDays(-numberOfDays);

            return await _dbContext.CodingSessions.
                Where(s => s.StartDate == targetDate)
                .OrderBy(s => s.StartDate)
                .ToListAsync();

        }


        public async Task<List<CodingSessionEntity>> GetTodayCodingSessionsAsync()
        {
            DateTime targetDate = DateTime.UtcNow;

            return await _dbContext.CodingSessions.
                Where(s => s.StartDate == targetDate)
                .OrderBy(s => s.StartDate)
                .ToListAsync();
        }


        public async Task<List<CodingSessionEntity>> GetAllCodingSessionAsync()
        {
            return await _dbContext.CodingSessions
                .ToListAsync();
        }

        public async Task<bool> CheckTodayCodingSessions()
        {
            var today = DateTime.UtcNow.Date;
            return await _dbContext.CodingSessions
                .AnyAsync(s => s.StartDate.HasValue &&
                               s.StartDate.Value.Date == today);
        }
    }
}
