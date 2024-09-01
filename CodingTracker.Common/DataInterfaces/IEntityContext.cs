using CodingTracker.Common.CodingSessions;
using CodingTracker.Common.UserCredentials;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.DataInterfaces.IEntityContexts
{
    public interface IEntityContext
    {
        DbSet<CodingSession> CodingSessions { get; }
        DbSet<UserCredential> UserCredentials { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
