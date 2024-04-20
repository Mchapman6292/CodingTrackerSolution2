using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IDatabaseSessionDeletes;

using CodingTracker.Common.IApplicationLoggers;

namespace CodingTracker.Data.DatabaseSessionDeletes
{
    public class DatabaseSessionDelete : IDatabaseSessionDelete
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseManager _databaseManager;
        private readonly CodingSessionDTO _codingSessionDTO;

        public DatabaseSessionDelete(IDatabaseManager databaseManager, CodingSessionDTO codingSessionDTO, IApplicationLogger appLogger)
        {
            _databaseManager = databaseManager;
            _codingSessionDTO = codingSessionDTO;
            _appLogger = appLogger;
        }

        public void DeleteSession(List<int> sessionIds)
        {
            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var transaction = connection.BeginTransaction();
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
                            command.Parameters.Clear();
                        }
                        catch (Exception ex)
                        {
                            _appLogger.Error($"Unable to delete session: SessionID {sessionId}. Error: {ex.Message}");
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _appLogger.Error($"Transaction failed and was rolled back. Error: {ex.Message}");
                }
            }, nameof(DeleteSession));
        }
    }
}
