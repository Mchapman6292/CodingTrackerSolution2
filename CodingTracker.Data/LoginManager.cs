using CodingTracker.Common.ILoginManagers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.ICredentialStorage;
using System.Data.SQLite;
using System.Data.SqlClient;
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.IApplicationLoggers;
using System.Diagnostics;


// resetPassword, updatePassword, rememberUser 
namespace CodingTracker.Data.LoginManagers
{
    public class LoginManager : ILoginManager
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseManager _databaseManager;
        private readonly ICredentialStorage _credentialStorage;
        public LoginManager(IApplicationLogger appLogger, IDatabaseManager databaseManager, ICredentialStorage credentialStorage)
        {
            _databaseManager = databaseManager;
            _credentialStorage = credentialStorage;
            _appLogger = appLogger;
        }


        public UserCredentialDTO ValidateLogin(string username, string password)
        {
            using (var activity = new Activity(nameof(ValidateLogin)).Start())
            {
                _appLogger.Info($"Starting {nameof(ValidateLogin)}. TraceID: {activity.TraceId}, Username: {username}");
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    var hashedPassword = _credentialStorage.HashPassword(password);

                    UserCredentialDTO userCredential = null; // Initialize as null to indicate no user found by default

                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using var command = connection.CreateCommand();
                        command.CommandText = @"
                SELECT 
                        UserId, Username, PasswordHash 
                FROM 
                        UserCredentials 
                WHERE 
                        Username = @Username";
                        command.Parameters.AddWithValue("@Username", username);

                        using var reader = command.ExecuteReader();
                        if (reader.Read()) // Used to check if there are any rows(users) returned by the query. 
                        {
                            var storedHash = reader["PasswordHash"].ToString();
                            if (hashedPassword == storedHash)
                            {
                                // Construct the DTO with data from the database, query against existing users. 
                                userCredential = new UserCredentialDTO
                                {
                                    UserId = Convert.ToInt32(reader["UserId"]),
                                    Username = reader["Username"].ToString(),
                                    Password = password
                                };

                            }
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
                    return userCredential; // Returns null if no matching user, or the UserCredentialDTO if a match is found
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(ValidateLogin)} for username {username}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
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
                                Users
                            SET 
                                PasswordHash = @HashedPassword
                            WHERE
                                Username = @Username";
                        command.Parameters.AddWithValue("@HashedPassword", hashedPassword);
                        command.Parameters.AddWithValue("@Username", username);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            _appLogger.Warning($"No user found with username {username}. Password reset failed. TraceID: {activity.TraceId}");
                        }
                        else
                        {
                            _appLogger.Info($"Password for user {username} has been successfully reset. Rows affected: {rowsAffected}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
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
