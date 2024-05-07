using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IDatabaseSessionDeletes;

using CodingTracker.Common.IApplicationLoggers;
using System.Data.SQLite;
using System.Diagnostics;

namespace CodingTracker.Data.DatabaseSessionDeletes
{
    public class DatabaseSessionDelete : IDatabaseSessionDelete
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseManager _databaseManager;

        public DatabaseSessionDelete(IDatabaseManager databaseManager, IApplicationLogger appLogger)
        {
            _databaseManager = databaseManager;
            _appLogger = appLogger;
        }

        public void DeleteSession(List<int> sessionIds)
        {
            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var transaction = connection.BeginTransaction();
                List<int> deletedSessionIds = new List<int>();  // List to keep track of successfully deleted session IDs
                try
                {
                    foreach (int sessionId in sessionIds)
                    {
                        try
                        {
                            using var command = connection.CreateCommand();
                            command.CommandText = @"
                        DELETE FROM
                                    CodingSessions
                          WHERE
                                    SessionId = @SessionId";

                            command.Parameters.AddWithValue("@SessionId", sessionId);
                            int affectedRows = command.ExecuteNonQuery();

                            if (affectedRows == 0)
                            {
                                _appLogger.Warning($"No session found with SessionID {sessionId}, nothing was deleted.");
                            }
                            else
                            {
                                deletedSessionIds.Add(sessionId);  // Add to the list if the deletion was successful
                            }
                            command.Parameters.Clear();
                        }
                        catch (Exception ex)
                        {
                            _appLogger.Error($"Unable to delete session: SessionID {sessionId}. Error: {ex.Message}");
                        }
                    }
                    transaction.Commit();
                    if (deletedSessionIds.Count > 0)
                    {
                        _appLogger.Info($"Deleted sessions with SessionIDs: {string.Join(", ", deletedSessionIds)}");
                    }
                    else
                    {
                        _appLogger.Info("No sessions were deleted.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _appLogger.Error($"Transaction failed and was rolled back. Error: {ex.Message}");
                }
            }, nameof(DeleteSession));
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
    }
}
