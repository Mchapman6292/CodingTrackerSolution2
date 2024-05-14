using CodingTracker.Common.CodingSessionDTOManagers;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.ICredentialManagers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.INewDatabaseReads;
using CodingTracker.Common.IQueryBuilders;
using CodingTracker.Common.UserCredentialDTOManagers;
using CodingTracker.Common.IAuthenticationServices;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.UserCredentialDTOs;
using System.Diagnostics;
using System.Data.SQLite;
using CodingTracker.Common.CurrentUserCredentials;

namespace CodingTracker.Business.AuthenticationServices
{
    public class AuthenticationService: IAuthenticationService
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseManager _databaseManager;
        private readonly ICredentialManager _credentialManager;
        private readonly CodingSessionDTO _codingSessionDTO;
        private readonly IUserCredentialDTOManager _userCredentialDTOManager;
        private readonly IQueryBuilder _queryBuilder;
        private readonly INewDatabaseRead _newDatabaseRead;
        private readonly ICodingSessionDTOManager _codingSessionDTOManager;
        private readonly CurrentUserCredentials _currentUserCredentials;

        public AuthenticationService(IApplicationLogger appLogger, IDatabaseManager databaseManager, ICredentialManager credentialManager, IUserCredentialDTOManager userCredentialDTOManager, IQueryBuilder queryBuilder, INewDatabaseRead newDatabaseRead, ICodingSessionDTOManager codingSessionDTOManager, CurrentUserCredentials currentUserCredentials)
        {
            _databaseManager = databaseManager;
            _credentialManager = credentialManager;
            _appLogger = appLogger;
            _userCredentialDTOManager = userCredentialDTOManager;
            _queryBuilder = queryBuilder;
            _newDatabaseRead = newDatabaseRead;
            _codingSessionDTOManager = codingSessionDTOManager;
        }


        public bool AuthenticateLogin(string username, string password)
        {
            bool isValid = false;

            // Use LogActivity method to encapsulate the entire operation
            _appLogger.LogActivity(nameof(AuthenticateLogin), activity =>
            {
                // Pre-action logging
                _appLogger.Info($"Starting {nameof(AuthenticateLogin)}. TraceID: {activity.TraceId}.");
            },
             activity =>
             {
                 // Main action
                 string hashedInputPassword = HashPassword(password);
                 List<UserCredentialDTO> credentials = _newDatabaseRead.HandleUserCredentialsOperations(
                     columnsToSelect: new List<string> { "UserId", "Username", "PasswordHash" },
                     username: username);

                 if (credentials.Any())
                 {
                     UserCredentialDTO currentCredentials = credentials.First();
                     _appLogger.Info($"UserCredentials read for {nameof(AuthenticateLogin)} UserId: {currentCredentials.UserId}, Username: {currentCredentials.Username}, PasswordHash: {currentCredentials.PasswordHash}. TraceID: {activity.TraceId}");

                     string storedHash = currentCredentials.PasswordHash;
                     isValid = storedHash == hashedInputPassword;

                     if (!isValid)
                     {
                         _appLogger.Info($"Password mismatch for {username}. StoredHash does not match input hash. TraceID: {activity.TraceId}");
                     }
                     else
                     {
                         _userCredentialDTOManager.UpdateCurrentUserCredentialDTO(currentCredentials);

                         _appLogger.Info($"UserId set for DTO managers {currentCredentials.UserId}, TraceID: {activity.TraceId}");

                     }
                 }
                 else
                 {
                     _appLogger.Info($"No credentials found for {username}. TraceID: {activity.TraceId}");
                 }
             },
            activity =>
            {
                _appLogger.Info($"{nameof(AuthenticateLogin)} complete for {username} isValid: {isValid}, TraceID: {activity.TraceId}");
            });

            return isValid;
        }

        public void ResetPassword(string username, string newPassword)
        {
            using (var activity = new Activity(nameof(ResetPassword)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ResetPassword)}. TraceID: {activity.TraceId}, Username: {username}");
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    string hashedPassword = _credentialManager.HashPassword(newPassword);

                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using var command = connection.CreateCommand();
                        command.CommandText = @"
                            UPDATE 
                                UserCredentials
                            SET 
                                PasswordHash = @HashedPassword
                            WHERE
                                Username = @Username";
                        command.Parameters.AddWithValue("@HashedPassword", hashedPassword);
                        command.Parameters.AddWithValue("@Username", username);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            _appLogger.Warning($"No user found with username {username}. PasswordHash reset failed. TraceID: {activity.TraceId}");
                        }
                        else
                        {
                            _appLogger.Info($"PasswordHash for user {username} has been successfully reset. Rows affected: {rowsAffected}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                        }
                    });
                    stopwatch.Stop();
                }
                catch (SQLiteException ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(ResetPassword)} for username {username}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
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

        public void UpdateCurrentUserId(int UserId)
        {
            _appLogger.LogActivity(nameof(UpdateCurrentUserId), activity =>
            {
                _appLogger.Info($"Starting {nameof(UpdateCurrentUserId)} _currentUserCredential.UserId = {_currentUserCredentials.UserId}. TraceID: {activity.TraceId}.");
            },
            activity =>
            {
                if (UserId == 0)
                {
                    _appLogger.Error($"UserId parameter of {nameof(UpdateCurrentUserId)} is {UserId}. 0 is the default for not set.");
                }
                else
                {
                    _currentUserCredentials.UserId = UserId;
                    _appLogger.Info($" _currentUserCredentials.UserId set to {_currentUserCredentials.UserId}. TraceID: {activity.TraceId}.");
                }
            });
        }

        public void UpdatePasswordHash(string passwordHash)
        {
            _appLogger.LogActivity(nameof(UpdatePasswordHash), activity =>
            {
                _appLogger.Info($"Starting {nameof(UpdatePasswordHash)} _currentUserCredentials.PasswordHash = {_currentUserCredentials.PasswordHash}. TraceID: {activity.TraceId}.");
            },
            activity =>
            {
                if (string.IsNullOrEmpty(passwordHash))
                {
                    _appLogger.Error($"PasswordHash parameter of {nameof(UpdatePasswordHash)} is null or empty. TraceID: {activity.TraceId}.");
                }
                else
                {
                    _currentUserCredentials.PasswordHash = passwordHash;
                    _appLogger.Info($" _currentUserCredentials.PasswordHash set to {_currentUserCredentials.PasswordHash}. TraceID: {activity.TraceId}.");
                }
            });
        }


        public void UpdateLastLogin(DateTime lastLogin)
        {
            _appLogger.LogActivity(nameof(UpdateLastLogin), activity =>
            {
                _appLogger.Info($"Starting {nameof(UpdateLastLogin)} _currentUserCredentials.LastLogin = {_currentUserCredentials.LastLogin}. TraceID: {activity.TraceId}.");
            },
            activity =>
            {
                if (lastLogin == DateTime.MinValue)
                {
                    _appLogger.Error($"LastLogin parameter of {nameof(UpdateLastLogin)} is set to DateTime.MinValue, which is the default for not set. TraceID: {activity.TraceId}.");
                }
                else
                {
                    _currentUserCredentials.LastLogin = lastLogin;
                    _appLogger.Info($" _currentUserCredentials.LastLogin updated to {_currentUserCredentials.LastLogin}. TraceID: {activity.TraceId}.");
                }
            });
        }
    }
}
