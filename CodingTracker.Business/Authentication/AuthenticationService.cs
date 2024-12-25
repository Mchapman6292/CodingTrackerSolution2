
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IAuthenticationServices;
using CodingTracker.Common.IQueryBuilders;
using CodingTracker.Common.UserCredentials;
using CodingTracker.Data.Repositories.UserCredentialRepositories;
using CodingTracker.Common.DataInterfaces.IUserCredentialRepositories;
using CodingTracker.Common.UserCredentials.UserCredentialDTOs;
using CodingTracker.Common.IUtilityServices;
using System.Data.SQLite;
using System.Diagnostics;
using CodingTracker.Business.CodingSessionService.UserIdServices;
using CodingTracker.Common.Entities.UserCredentialEntities;


// resetPassword, updatePassword, rememberUser 
namespace CodingTracker.Business.Authentication.AuthenticationServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IQueryBuilder _queryBuilder;
        private readonly IUserCredentialRepository _userCredentialRepository;
        private readonly IUtilityService _utilityService;
        private readonly UserIdService _userIdService;

        public AuthenticationService(IApplicationLogger appLogger,IQueryBuilder queryBuilder, IUserCredentialRepository userCredentialRepository, IUtilityService utilityService, UserIdService userIdService)
        {
            _appLogger = appLogger;
            _queryBuilder = queryBuilder;
            _userCredentialRepository = userCredentialRepository;
            _utilityService = utilityService;
            _userIdService = userIdService;
        }


        public async Task<bool> CreateAccount(string username, string password)
        {
            if(await _userCredentialRepository.UsernameExistsAsync(username))
            {
                _appLogger.Error($"Username already exists.");
            }

            UserCredentialEntity user = new UserCredentialEntity
            {
                Username = username,
                PasswordHash = _utilityService.HashPassword(password),
                LastLogin = DateTime.Now
                
            };

            return await _userCredentialRepository.AddUserCredentialAsync(user);

        }

        public async Task<bool> AuthenticateLogin(string username, string password, Activity activity)
        {
            bool usernameExist = await _userCredentialRepository.UsernameExistsAsync(username);

            if (!usernameExist)
            {
                _appLogger.Info($"No username exists for {username}");
                return false;
            }


            UserCredentialEntity loginCredential = await _userCredentialRepository.GetUserCredentialByUsernameAsync(username);

            var inputHash = _utilityService.HashPassword(password);
            var storedHash = loginCredential.PasswordHash;

            bool isValid = inputHash.Equals(storedHash, StringComparison.Ordinal);

            if (!isValid)
            {
                return false;
            }

            _userIdService.SetCurrentUserId(loginCredential.UserId);
            return true;
        }


        public async Task<bool> ResetPassword(string username, string newPassword)
        {
            if (! await _userCredentialRepository.UsernameExistsAsync(username))
            {
                _appLogger.Info($"Password reset failed: No user found for username {username}");
                return false;
            }

            string hashedPassword = _utilityService.HashPassword(newPassword);

            return await _userCredentialRepository.UpdatePassWord(username, hashedPassword);
         

        }
    }
}

