using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IDatabaseSessionUpdates;
using CodingTracker.Data.DatabaseManagers;
using CodingTracker.Common.ICredentialManagers;

using CodingTracker.Common.ICredentialManagers;

namespace CodingTracker.Data
{
    public class DatabaseSessionUpdate : IDatabaseSessionUpdate
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseManager _databaseManager;
        private readonly ICredentialManager _credentialManager;

        public DatabaseSessionUpdate(IApplicationLogger appLogger, IDatabaseManager databaseManager, ICredentialManager credentialManager)
        {
            _appLogger = appLogger;
            _databaseManager = databaseManager;
            _credentialManager = credentialManager;
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
                string hashedPassword = _credentialManager.HashPassword(newPassword);
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
                        _appLogger.Info($"PasswordHash updated successfully for UserId {userId}. Rows affected: {affectedRows}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }
                    catch (SQLiteException ex)
                    {
                        _appLogger.Error($"Failed to update password for UserId {userId}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    }
                });
            }
        }


    }
}
