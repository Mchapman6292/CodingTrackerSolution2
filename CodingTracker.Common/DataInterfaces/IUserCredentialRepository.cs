using CodingTracker.Common.UserCredentials;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.DataInterfaces.IUserCredentialRepository
{
    public interface IUserCredentialRepository
    {
        Task<UserCredential> GetCredentialByUsername(string username, Activity activity);


        Task<bool> AddUserCredential(Activity activity, UserCredential newUser);

        Task<bool> UpdateUserCredentialPassword(string username, string password, Activity activity);

        Task<bool> UpdateLastLogin(string username, DateTime lastLogin, Activity activity);

        Task<bool> SaveUserCredentialChanges(Activity activity);
    }
}
