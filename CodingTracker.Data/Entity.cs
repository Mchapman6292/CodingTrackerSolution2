using CodingTracker.Common.CodingSessionDTOManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.CodingSessions;
using CodingTracker.Common.UserCredentials;
using Microsoft.EntityFrameworkCore;

namespace CodingTracker.Data.EntityContexts
{ 
    public class EntityContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<CodingSession> codingSessions { get; set; }
        public DbSet<UserCredential> userCredentials { get; set; }

        public string DbPath { get; }

        public EntityContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "blogging.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={DbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CodingSession>()
                .HasKey(cs => cs.SessionId);            // HasKey is used to specify the primary key for the entity. 
                                                        // cs represents an instance of CodingSesssion.
                                                        // cs.SessionId is the property that should be used as the primary key.
                                                        // Lambda function takes a CodingSession object and returns it SessionId.
            modelBuilder.Entity<UserCredential>()
                    .HasKey(uc => uc.UserId);

            modelBuilder.Entity<CodingSession>()
                .HasOne(cs => cs.User)
                .WithMany(u => u.CodingSessions)
                .HasForeignKey(cs => cs.UserId);

            modelBuilder.Entity<UserCredential>()
                .HasIndex(u => u.Username)              // Adding an index makes having a unique username a requirement.
                .IsUnique();
        }


    }
}

