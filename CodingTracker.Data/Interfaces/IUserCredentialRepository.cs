using CodingTracker.Common.UserCredentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Data.Interfaces.IUserCredentialRepository
{
    public interface IUserCredentialRepository
    {
        Task<UserCredential> GetCredentialByUsername(string username, string traceId, string parentId);
    }
}
