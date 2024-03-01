using System;
using System.Text;
using System.Security.Cryptography;
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.ICredentialStorage;
using CodingTracker.Data.CredentialServices;
using CodingTracker.Common.ICredentialServices;
using CodingTracker.Common.IDatabaseManagers;
using System.Data.SQLite;
using CodingTracker.Common.IApplicationLoggers;
using System.Diagnostics;
using System.Net;


// Pass DTO as parameter to methods that act on multiple properties
namespace CodingTracker.Data.CredentialStorage
{
    public class CredentialStorage : ICredentialStorage
    {
        private readonly IApplicationLogger _appLogger;
        private readonly ICredentialService _credentialService;
        private readonly IDatabaseManager _databaseManager;

        public CredentialStorage(IApplicationLogger logger, ICredentialService credentialService, IDatabaseManager databaseManager)
        {
            _appLogger = logger;
            _credentialService = credentialService;
            _databaseManager = databaseManager;
        }

        public void AddCredentials(UserCredentialDTO credential)
        {
            using (var activity = new Activity(nameof(AddCredentials)).Start()) // Start a new activity
            {
                _appLogger.Debug($"Starting {nameof(AddCredentials)}. TraceID: {activity.TraceId}, UserId: {credential.UserId}, Username: {credential.Username}");

                string hashedPassword = HashPassword(credential.Password);
                _databaseManager.ExecuteCRUD(connection =>
                {
                    using var command = new SQLiteCommand(@"
                    INSERT INTO
                        UserCredentials
                    (
                        UserId,
                        Username,
                        PasswordHash
                    )
                    VALUES
                    (
                        @UserId,
                        @Username,
                        @PasswordHash
                    )"
                                , connection);

                    command.Parameters.AddWithValue("@UserId", credential.UserId);
                    command.Parameters.AddWithValue("@Username", credential.Username);
                    command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                    try
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        int affectedRows = command.ExecuteNonQuery();
                        stopwatch.Stop();
                        _appLogger.Info($"Credentials added successfully for {credential.Username}. Rows affected: {affectedRows}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }
                    catch (SQLiteException ex)
                    {
                        _appLogger.Error($"Failed to add credentials for {credential.Username}. Error: {ex.Message}. TraceID: {Activity.Current?.TraceId}");
                    }
                });
            }
        }

        public void UpdateCredentials(int userId, string newUsername, string newPassword)
        {
            UpdateUserName(userId, newUsername);
            UpdatePassword(userId, newPassword);
        }

        public void UpdateUserName(int userId, string newUserName)
        {
            using (var activity = new Activity(nameof(UpdateUserName)).Start())
            {
                _appLogger.Debug($"Starting {nameof(UpdateUserName)}. TraceID: {activity.TraceId}, UserId: {userId}, NewUserName: {newUserName}");
                _databaseManager.ExecuteCRUD(connection =>
                {
                    using var command = new SQLiteCommand(@"
                    UPDATE
                        UserCredentials
                    SET
                        Username = @Username
                    WHERE
                        UserId = @UserId",

                            connection);

                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Username", newUserName);

                    try
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        int affectedRows = command.ExecuteNonQuery();
                        stopwatch.Stop();
                        _appLogger.Info($"Username updated successfully for UserId {userId}. Rows affected: {affectedRows}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }
                    catch (SQLiteException ex)
                    {
                        _appLogger.Error($"Failed to update username for UserId {userId}. Error: {ex.Message}. TraceID: {activity.TraceId}");
                    }
                });
            }
        }


        public void UpdatePassword(int userId, string newPassword)
        {
            using (var activity = new Activity(nameof(UpdatePassword)).Start())
            {
                _appLogger.Debug($"Starting {nameof(UpdatePassword)}. TraceID: {activity.TraceId}, UserId: {userId}");
                string hashedPassword = HashPassword(newPassword);
                _databaseManager.ExecuteCRUD(connection =>
                {
                    using var command = new SQLiteCommand(@"
                UPDATE
                    UserCredentials
                SET 
                    PasswordHash = @PasswordHash
                WHERE
                    UserId = @UserId",
                        connection);

                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                    try
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        int affectedRows = command.ExecuteNonQuery();
                        stopwatch.Stop();
                        _appLogger.Info($"Password updated successfully for UserId {userId}. Rows affected: {affectedRows}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }
                    catch (SQLiteException ex)
                    {
                        _appLogger.Error($"Failed to update password for UserId {userId}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    }
                });
            }
        }

        public void DeleteCredentials(int userId)
        {
            using (var activity = new Activity(nameof(DeleteCredentials)).Start())
            {
                _appLogger.Debug($"Starting {nameof(DeleteCredentials)}. TraceID: {activity.TraceId}, UserId: {userId}");
                _databaseManager.ExecuteCRUD(connection =>
                {
                    using var command = new SQLiteCommand(@"
                DELETE FROM 
                    UserCredentials
                WHERE
                    UserId = @UserId",
                        connection);

                    command.Parameters.AddWithValue("@UserId", userId);

                    try
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        int affectedRows = command.ExecuteNonQuery();
                        stopwatch.Stop();
                        _appLogger.Info($"Credentials deleted successfully for UserId {userId}. Rows affected: {affectedRows}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }
                    catch (SQLiteException ex)
                    {
                        _appLogger.Error($"Failed to delete credentials for UserId {userId}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    }
                });
            }
        }


            /// <summary>
            /// Does not create a new activity as exceptions thrown here are re-thrown and handled by the caller, where they are logged within the appropriate operational context.
            /// </summary>
            /// </summary>
            /// <param name="password"></param>
            /// <returns></returns>
            public string HashPassword(string password)
        {
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
                    return builder.ToString();
                }
            }
            catch (ArgumentNullException ex)
            {
                _appLogger.Error("Password cannot be null.", ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                _appLogger.Error("An unexpected error occurred while hashing the password.", ex);
                throw;
            }
            catch (Exception ex)
            {
                _appLogger.Error("An error occurred while processing your request.", ex);
                throw;
            }
        }

        UserCredentialDTO GetCredentialById(int userId)//needed?
        {
            throw new NotImplementedException(" not implemented.");
        }
    }
}


