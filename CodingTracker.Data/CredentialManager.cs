using System;
using System.Text;
using System.Security.Cryptography;
using CodingTracker.Common.ICredentialManagers;
using System.Data.SQLite;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.DataInterfaces.IUserCredentialRepositories;
using CodingTracker.Common.IUtilityServices;
using System.Diagnostics;
using CodingTracker.Common.UserCredentials.UserCredentialDTOs;

using System.Net;
using CodingTracker.Common.UserCredentials;
using CodingTracker.Data.Repositories.UserCredentialRepositories;


// Pass DTO as parameter to methods that act on multiple properties
namespace CodingTracker.Data.CredentialManagers
{
    public class CredentialManager : ICredentialManager
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IUserCredentialRepository _userCredentialRepository;
        private readonly IUtilityService _utilityService;


        public CredentialManager(IApplicationLogger applogger, UserCredentialRepository userCredentialRepository, IUtilityService utilityService)
        {
            _appLogger = applogger;
            _userCredentialRepository = userCredentialRepository;
            _utilityService = utilityService;
        }

        public async Task<bool> CreateAccount(Activity activity, string username, string password)
        {
            try
            {
                _appLogger.Info($"Starting {nameof(CreateAccount)}. TraceID: {activity.TraceId}, Username: {username}");

                string paswordHash = _utilityService.HashPassword(activity, password);

                UserCredentialDTO newCredential = new UserCredentialDTO
                {
                    Username = username,
                    PasswordHash = paswordHash
                };


               bool isAdded = await _userCredentialRepository.AddUserCredential(activity, newCredential);

                if (isAdded) 
                {
                    _appLogger.Info($"Account created successfully. TraceID: {activity.TraceId}, Username: {username}");
                    return true;
                }
                else
                {
                    _appLogger.Warning($"Failed to create account. TraceID: {activity.TraceId}, Username: {username}");
                    return false;
                }
            }
            catch (Exception ex) 
            {
                _appLogger.Error($"Unexpected error during {nameof(CreateAccount)}. TraceID: {activity.TraceId}, Username: {username}", ex);
                return false;
            }
        }





        private bool checkifCredentialsExist()
        {
            throw new NotImplementedException();
        }



       






    }
}


