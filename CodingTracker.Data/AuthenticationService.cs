using CodingTracker.Common.ILoginManagers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.ICredentialManagers;
using System.Data.SQLite;
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.IApplicationLoggers;
using System.Diagnostics;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.UserCredentialDTOManagers;
using CodingTracker.Common.IQueryBuilders;
using CodingTracker.Data.NewDatabaseReads;
using System.Runtime.CompilerServices;
using CodingTracker.Common.INewDatabaseReads;
using System.Text;
using System.Security.Cryptography;
using CodingTracker.Common.CodingSessionDTOManagers;


// resetPassword, updatePassword, rememberUser 
namespace CodingTracker.Common.IAuthenticationServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseManager _databaseManager;
        private readonly ICredentialManager _credentialManager;
        private readonly CodingSessionDTO _codingSessionDTO;
        private readonly IUserCredentialDTOManager _userCredentialDTOManager;
        private readonly IQueryBuilder _queryBuilder;
        private readonly INewDatabaseRead _newDatabaseRead;
        private readonly ICodingSessionDTOManager _codingSessionDTOManager;

        private int _currentUserId;
        public AuthenticationService(IApplicationLogger appLogger, IDatabaseManager databaseManager, ICredentialManager credentialManager, IUserCredentialDTOManager userCredentialDTOManager, IQueryBuilder queryBuilder, INewDatabaseRead newDatabaseRead, ICodingSessionDTOManager codingSessionDTOManager)
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
                _appLogger.Debug($"Starting {nameof(AuthenticateLogin)}. TraceID: {activity.TraceId}.");
            },
             activity =>
            {
                // Main action
                string hashedInputPassword = HashPassword(password);
                List<UserCredentialDTO> credentials = _newDatabaseRead.HandleUserCredentialsOperations(
                    columnsToSelect: new List<string> { "userId", "PasswordHash" },
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
                        _userCredentialDTOManager.SetCurrentUserCredential(currentCredentials);
                        _userCredentialDTOManager.AssignCurrentUserId(currentCredentials.UserId);
                        _codingSessionDTOManager.SetCodingSessionUserId(currentCredentials.UserId);

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






        public UserCredentialDTO GetUserDetails(string username)
        {
            UserCredentialDTO userCredential = null;

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                var commandText = _queryBuilder.CreateCommandTextForUserCredentials(
                    new List<string> { "userId", "Username", "PasswordHash", "LastLogin" }, username: username);

                using (var command = new SQLiteCommand(commandText, connection))
                {
                    _queryBuilder.SetCommandParametersForUserCredentials(command, username: username);

                    using var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        userCredential = new UserCredentialDTO
                        {
                            UserId = Convert.ToInt32(reader["userId"]),
                            Username = reader["Username"].ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),
                            LastLogin = reader.IsDBNull(reader.GetOrdinal("LastLogin")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("LastLogin"))
                        };
                    }
                }
            }, nameof(GetUserDetails));

            return userCredential;
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
    }
}
