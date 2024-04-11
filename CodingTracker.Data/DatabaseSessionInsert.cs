using CodingTracker.Common;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IDatabaseSessionInserts;
using System.Data.SQLite;
using CodingTracker.Data.DatabaseManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using CodingTracker.Common.CodingSessionDTOManagers;

namespace CodingTracker.Data.DatabaseSessionInserts
{
    public class DatabaseSessionInsert : IDatabaseSessionInsert
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IApplicationLogger _appLogger;
        private readonly CodingSessionDTO _codingSessionDTO;
        private readonly ICodingSessionDTOManager _codingSessionDTOManager;




        public DatabaseSessionInsert(IDatabaseManager databaseManager, IApplicationLogger appLogger, ICodingSessionDTOManager codingSessionDTOManager)
        {
            _databaseManager = databaseManager;
            _appLogger = appLogger;
            _codingSessionDTOManager = codingSessionDTOManager;
        }


        public void InsertSession()
        {
            CodingSessionDTO codingSessionDTO = _codingSessionDTOManager.GetCurrentSessionDTO();
            using (var activity = new Activity(nameof(InsertSession)).Start())
            {
                _appLogger.Debug($"Starting {nameof(InsertSession)}. TraceID: {activity.TraceId}, UserId: {codingSessionDTO.UserId}");

                Stopwatch stopwatch = Stopwatch.StartNew();

                _databaseManager.ExecuteCRUD(connection =>
                {
                    using var command = connection.CreateCommand();
                    command.CommandText = @"
                        INSERT INTO CodingSessions 
                        (
                            UserId, 
                            StartTime, 
                            EndTime,
                            DurationMinutes
                        ) 
                        VALUES 
                        (
                            @UserId, 
                            @StartTime, 
                            @EndTime, 
                            @DurationMinutes

                        )";

                    command.Parameters.AddWithValue("@UserId", codingSessionDTO.UserId);
                    command.Parameters.AddWithValue("@StartTime", codingSessionDTO.StartTime);
                    command.Parameters.AddWithValue("@EndTime", codingSessionDTO.EndTime.HasValue ? (object)codingSessionDTO.EndTime.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@DurationMinutes", codingSessionDTO.DurationMinutes);


                    try
                    {
                        int affectedRows = command.ExecuteNonQuery();
                        stopwatch.Stop();
                        _appLogger.Info($"Session inserted successfully for UserId: {codingSessionDTO.UserId}. Rows affected: {affectedRows}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }
                    catch (SQLiteException ex)
                    {
                        stopwatch.Stop();
                        _appLogger.Error($"Failed to insert session for UserId: {codingSessionDTO.UserId}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                        Console.WriteLine("Failed to insert session");
                    }
                });
            }
        }
    }
}
