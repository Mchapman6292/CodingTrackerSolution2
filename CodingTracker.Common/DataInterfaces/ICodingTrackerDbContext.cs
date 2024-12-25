using Microsoft.EntityFrameworkCore;
using CodingTracker.Common.Entities.UserCredentialEntities;
using CodingTracker.Common.Entities.CodingSessionEntities;

namespace CodingTracker.Common.DataInterfaces.ICodingTrackerDbContexts
{
    public interface ICodingTrackerDbContext
    {
        DbSet<CodingSessionEntity> CodingSessions { get; set; }
        DbSet<UserCredentialEntity> UserCredentials { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
