using Microsoft.EntityFrameworkCore;
using CodingTracker.Common.Entities.UserCredentialEntities;
using CodingTracker.Common.Entities;
using CodingTracker.Common.UserCredentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.DataInterfaces.ICodingTrackerDbContexts
{
    public interface ICodingTrackerDbContext
    {
        DbSet<CodingSessionEntity> CodingSessions { get; set; }
        DbSet<UserCredentialEntity> UserCredentials { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
