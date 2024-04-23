using CodingTracker.Common.ILoginManagers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.ICredentialManagers;
using System.Data.SQLite;
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.IApplicationLoggers;
using System.Diagnostics;
using CodingTracker.Common.CodingSessionDTOs;


// resetPassword, updatePassword, rememberUser 
namespace CodingTracker.Data.LoginManagers
{
    public class LoginManager : ILoginManager
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseManager _databaseManager;
        private readonly ICredentialManager _credentialStorage;
        private readonly CodingSessionDTO _codingSessionDTO;
        public LoginManager(IApplicationLogger appLogger, IDatabaseManager databaseManager, ICredentialManager credentialStorage)
        {
            _databaseManager = databaseManager;
            _credentialStorage = credentialStorage;
            _appLogger = appLogger;
        }


        public UserCredentialDTO ValidateLogin(string username, string password)
        {
            var lastLogin = string.Empty;
            using (var activity = new Activity(nameof(ValidateLogin)).Start())
            {
                _appLogger.Info($"Starting {nameof(ValidateLogin)}. TraceID: {activity.TraceId}, Username: {username}");
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    _appLogger.Debug($"Hashing password for {username}. TraceID: {activity.TraceId}");
                    var hashedPassword = _credentialStorage.HashPassword(password);
                    _appLogger.Debug($"PasswordHash hashed for {username}. TraceID: {activity.TraceId}");

                    UserCredentialDTO userCredential = null;

                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using var command = connection.CreateCommand();
                        command.CommandText = @"
                                SELECT 
                                        UserId, 
                                        Username, 
                                        PasswordHash,
                                        LastLogin
                                FROM 
                                        UserCredentials
                                WHERE 
                                        Username = @Username";

                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@PasswordHash", password);
                        command.Parameters.AddWithValue("@LastLogin", lastLogin);

                        _appLogger.Debug($"Executing database query for {username}. TraceID: {activity.TraceId}");


                        using var reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            _appLogger.Debug($"User record found for {username}. TraceID: {activity.TraceId}");

                            var storedHash = reader["PasswordHash"].ToString();
                            if (hashedPassword == storedHash)
                            {
                                _appLogger.Debug($"PasswordHash match for {username}. TraceID: {activity.TraceId}");

                                userCredential = new UserCredentialDTO
                                {
                                    UserId = Convert.ToInt32(reader["UserId"]),
                                    Username = reader["Username"].ToString(),
                                    PasswordHash = password
                                };
                            }
                            else
                            {
                                _appLogger.Info($"PasswordHash mismatch for {username}. TraceID: {activity.TraceId}");
                            }
                        }
                        else
                        {
                            _appLogger.Info($"No user record found for {username}. TraceID: {activity.TraceId}");
                        }
                    });

                    stopwatch.Stop();
                    if (userCredential != null)
                    {
                        _appLogger.Info($"User {username} validated successfully. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }
                    else
                    {
                        _appLogger.Info($"User {username} validation failed. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }

                    return userCredential; // Returns null if no matching user 
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(ValidateLogin)} for username {username}. Error: {ex.GetType().Name} - {ex.Message}. TraceID: {activity.TraceId}", ex);
                    return null;
                }
            }
        }



        public void ResetPassword(string username, string newPassword)
        {
            using (var activity = new Activity(nameof(ResetPassword)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ResetPassword)}. TraceID: {activity.TraceId}, Username: {username}");
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    string hashedPassword = _credentialStorage.HashPassword(newPassword);

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
    }
}
