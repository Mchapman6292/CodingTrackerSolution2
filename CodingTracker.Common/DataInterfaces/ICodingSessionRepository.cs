using CodingTracker.Common.Entities.CodingSessionEntities;

namespace CodingTracker.Common.DataInterfaces.ICodingSessionRepositories
{
    public interface ICodingSessionRepository
    {
        Task<List<CodingSessionEntity>> GetSessionsbyIDAsync(List<int> sessionIds);

        Task<int> DeleteSessionsByIdAsync(IReadOnlyCollection<int> sessionIds);

        Task<List<CodingSessionEntity>> GetRecentSessionsAsync(int numberOfSessions);

        Task<List<CodingSessionEntity>> GetSessionsForLastDaysAsync(int numberOfDays);

        Task<List<CodingSessionEntity>> GetTodayCodingSessionsAsync();

        Task<List<CodingSessionEntity>> GetAllCodingSessionAsync();

        Task<bool> CheckTodayCodingSessions();
    }
}
