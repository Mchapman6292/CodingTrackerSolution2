
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.ICredentialManagers;
using CodingTracker.Common.IAuthtenticationServices;
using CodingTracker.Common.IQueryBuilders;
using CodingTracker.Common.UserCredentials;
using CodingTracker.Common.DataInterfaces.IUserCredentialRepository;
using CodingTracker.Common.IUtilityServices;
using System.Data.SQLite;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;


// resetPassword, updatePassword, rememberUser 
namespace CodingTracker.Common.IAuthenticationServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IApplicationLogger _appLogger;
        private readonly ICredentialManager _credentialManager;
        private readonly IQueryBuilder _queryBuilder;
        private readonly IUserCredentialRepository _userCredentialRepository;
        private readonly IUtilityService _utilityService;

        private int _currentUserId{ get; set; }
        public AuthenticationService(IApplicationLogger appLogger,ICredentialManager credentialManager,  IQueryBuilder queryBuilder, IUserCredentialRepository userCredentialRepository, IUtilityService utilityService)
        {
            _credentialManager = credentialManager;
            _appLogger = appLogger;
            _queryBuilder = queryBuilder;
            _userCredentialRepository = userCredentialRepository;
            _utilityService = utilityService;
        }


        public async Task<bool> AuthenticateLogin(string username, string password, Activity activity)
        {
            _appLogger.Info($"Starting {nameof(AuthenticateLogin)} TraceId: {activity.TraceId}, ParentId: {activity.ParentId}. ");
            try
            {
                UserCredentialDTO loginCredential = await _userCredentialRepository.GetCredentialByUsername(username, activity);

 

                var inputHash = _utilityService.HashPassword(activity ,password);
                var storedHash = loginCredential.PasswordHash;

                bool isValid = inputHash.Equals(storedHash, StringComparison.Ordinal);

                if (!isValid)
                {
                    _appLogger.Info($" Error during {nameof(AuthenticateLogin)} inputHash and storedHash are the not the same. TraceId: {activity.TraceId}, ParentId: {activity.ParentId}.");
                    return false;
                }

                    _appLogger.Info($"{nameof(AuthenticateLogin)} successful. TraceId: {activity.TraceId}, ParentId: {activity.ParentId}.");
                    _currentUserId = loginCredential.UserId;
                    return true;
                   
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error in {nameof(AuthenticateLogin)} TraceId: {activity.TraceId}, ParentId: {activity.ParentId}.", ex);
                return false;
            }
        }

        public async Task<int> GetCurrentUserId(Activity activity)
        {
            _appLogger.Info($"Starting {nameof(GetCurrentUserId)} TraceId: {activity.TraceId}.");


            if (_currentUserId == 0)
            {
                _appLogger.Error($"CurrentUserId is not set currentUserId: {_currentUserId}, TraceId: {activity.TraceId}.");
                return _currentUserId;
            }
            _appLogger.Info($"CurrentUserId returned :{_currentUserId}, TraceId: {activity.TraceId}.");
            return _currentUserId;
        }


   





        public async Task<bool> ResetPassword(string username, string newPassword)
        {
            using (var activity = new Activity(nameof(ResetPassword)).Start())
            {
                _appLogger.Info($"Starting {nameof(ResetPassword)}. TraceID: {activity.TraceId}, Username: {username}");
                try
                {
                    UserCredentialDTO user = await _userCredentialRepository.GetCredentialByUsername(username, activity);
                    if (user == null)
                    {
                        _appLogger.Warning($"User not found for password reset: {username}. TraceID: {activity.TraceId}");
                        return false;
                    }

                    string hashedNewPassword = _utilityService.HashPassword(activity, newPassword);
                    user.PasswordHash = hashedNewPassword;

                    bool updateResult = await _userCredentialRepository.UpdateUserCredentialPassword(username, newPassword, activity);
                    if (updateResult)
                    {
                        _appLogger.Info($"Password reset successfully for User: {username}, TraceID: {activity.TraceId}");
                        return true;
                    }
                    else
                    {
                        _appLogger.Warning($"Failed to reset password for User: {username}, TraceID: {activity.TraceId}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"Error during password reset for user: {username}. TraceID: {activity.TraceId}", ex);
                    return false;
                }
            }
        }

    }
}

