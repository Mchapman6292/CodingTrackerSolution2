using System;
using System.Text;
using System.Security.Cryptography;
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.ICredentialManagers;
using CodingTracker.Common.IDatabaseManagers;
using System.Data.SQLite;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.DataInterfaces.IUserCredentialRepository;
using CodingTracker.Common.IUtilityServices;
using System.Diagnostics;

using System.Net;
using CodingTracker.Common.UserCredentials;


// Pass DTO as parameter to methods that act on multiple properties
namespace CodingTracker.Data.CredentialManagers
{
    public class CredentialManager : ICredentialManager
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseManager _databaseManager;
        private readonly IUserCredentialRepository _userCredentialRepository;
        private readonly IUtilityService _utilityService;


        public CredentialManager(IApplicationLogger applogger,  IDatabaseManager databaseManager, IUserCredentialRepository userCredentialRepository, IUtilityService utilityService)
        {
            _appLogger = applogger;
            _databaseManager = databaseManager;
            _userCredentialRepository = userCredentialRepository;
            _utilityService = utilityService;
        }

        public async Task<bool> CreateAccount(Activity activity, string username, string password)
        {
            try
            {
                _appLogger.Info($"Starting {nameof(CreateAccount)}. TraceID: {activity.TraceId}, Username: {username}");

                string paswordHash = _utilityService.HashPassword(activity, password);

                UserCredential newCredential = new UserCredential
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


        public bool IsAccountCreatedSuccessfully(string username)
        {
            using (var activity = new Activity(nameof(IsAccountCreatedSuccessfully)).Start())
            {
                _appLogger.Debug($"Starting {nameof(IsAccountCreatedSuccessfully)}. TraceID: {activity.TraceId}, Username: {username}");

                Stopwatch stopwatch = Stopwatch.StartNew();

                bool isCreated = false;
                _databaseManager.ExecuteCRUD(connection =>
                {
                    using var command = new SQLiteCommand(@"
                SELECT EXISTS(SELECT 1 FROM UserCredentials WHERE Username = @Username)",
                        connection);

                    command.Parameters.AddWithValue("@Username", username);

                    try
                    {
                        isCreated = Convert.ToBoolean(command.ExecuteScalar());
                        stopwatch.Stop();
                        _appLogger.Info($"{nameof(IsAccountCreatedSuccessfully)} completed for {username}. Account exists: {isCreated}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }
                    catch (SQLiteException ex)
                    {
                        stopwatch.Stop();
                        _appLogger.Error($"Failed to check if account was created for {username}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    }
                });

                return isCreated;
            }
        }


       


        public string HashPassword(string password) 
        {
            using (var activity = new Activity(nameof(HashPassword)).Start())
            {
                _appLogger.Debug($"Starting {nameof(HashPassword)}, TraceId: {activity.TraceId}.");
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    using (SHA256 sha256Hash = SHA256.Create())
                    {
                        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                        StringBuilder builder = new StringBuilder();
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            builder.Append(bytes[i].ToString("x2"));
                        }

                        stopwatch.Stop();
                        _appLogger.Info($"{nameof(HashPassword)} completed successfully. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}.");
                        return builder.ToString();
                    }
                }
                catch (ArgumentNullException ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Password cannot be null. Error: {ex.Message}. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    throw;
                }
                catch (EncoderFallbackException ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Encoding error while hashing the password. Error: {ex.Message}. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    throw;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"An unexpected error occurred while hashing the password. Error: {ex.Message}. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    throw;
                }
            }
        }




        public UserCredentialDTO GetCredentialById(int userId)//needed?
        {
            throw new NotImplementedException(" not implemented.");
        }
    }
}


