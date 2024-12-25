using CodingTracker.Common.Entities.UserCredentialEntities;
using CodingTracker.Common.UserCredentials.UserCredentialDTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.DataInterfaces.IUserCredentialRepositories
{
    public interface IUserCredentialRepository
    {
        Task<bool> UserIdExistsAsync(int userId);
        Task<UserCredentialEntity?> GetUserCredentialByIdAsync(int userId);
        Task<bool> UsernameExistsAsync(string username);
        Task<UserCredentialEntity?> GetUserCredentialByUsernameAsync(string username);
        Task<bool> AddUserCredentialAsync(UserCredentialEntity userCredential);
        Task<bool> ValidateUserCredentialsAsync(string username, string hashedPassword);
        Task<bool> UpdateUserCredentialsAsync(string username, string passwordHash, int userId);
        Task<bool> UpdatePassWord(string username, string hashedPassword);
        Task<bool> DeleteUserAsync(int userId);
    }
}
