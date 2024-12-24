using Microsoft.EntityFrameworkCore;
using CodingTracker.Common.DataInterfaces.ICodingTrackerDbContexts;
using CodingTracker.Common.Entities.UserCredentialEntities;
using CodingTracker.Common.Entities;


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





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<CodingSessionEntity>()
                .HasOne<UserCredentialEntity>()
                .WithMany()
                .HasForeignKey(e => e.UserId);


            modelBuilder.Entity<UserCredentialEntity>()
                .HasIndex(e => e.Username)
                .IsUnique();

        }
    }
}
