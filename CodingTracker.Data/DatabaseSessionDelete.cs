using CodingTracker.Common;
using System.Data.SQLite;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IDatabaseSessionDeletes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
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




        public void DeleteSession(CodingSessionDTO codingSessionDTO)
        {
            using (var activity = new Activity(nameof(DeleteSession)).Start())
            {
                _appLogger.Debug($"Starting {nameof(DeleteSession)}. TraceID: {activity.TraceId}, SessionId: {codingSessionDTO.SessionId}, UserId: {codingSessionDTO.UserId}");

                _databaseManager.ExecuteCRUD(connection =>
                {
                    using var command = connection.CreateCommand();
                    command.CommandText = @"
                DELETE FROM CodingSessions 
                WHERE 
                    SessionId = @SessionId AND 
                    UserId = @UserId";

                    command.Parameters.AddWithValue("@SessionId", codingSessionDTO.SessionId);
                    command.Parameters.AddWithValue("@UserId", codingSessionDTO.UserId);

                    try
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        int affectedRows = command.ExecuteNonQuery();
                        stopwatch.Stop();
                        _appLogger.Info($"Session deleted successfully. SessionId: {codingSessionDTO.SessionId}, UserId: {codingSessionDTO.UserId}. Rows affected: {affectedRows}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }
                    catch (SQLiteException ex)
                    {
                        _appLogger.Error($"Failed to delete session. SessionId: {codingSessionDTO.SessionId}, UserId: {codingSessionDTO.UserId}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                        Console.Write("Failed to delete session");
                    }
                });
            }
        }


    }
}
