using Microsoft.EntityFrameworkCore;
using CodingTracker.Common.DataInterfaces.ICodingTrackerDbContexts;
using CodingTracker.Common.Entities.UserCredentialEntities;
using CodingTracker.Common.Entities.CodingSessionEntities;


namespace CodingTracker.Data.DbContextService.CodingTrackerDbContexts
{
    public class CodingTrackerDbContext : DbContext, ICodingTrackerDbContext
    {
        public CodingTrackerDbContext(DbContextOptions<CodingTrackerDbContext> options)
            : base(options)
        {

        }
        public DbSet<CodingSessionEntity> CodingSessions { get; set; }
        public DbSet<UserCredentialEntity> UserCredentials { get; set; }




    }
}
