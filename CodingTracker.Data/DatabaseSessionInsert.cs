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

namespace CodingTracker.Data.DatabaseSessionInserts
{
    public class DatabaseSessionInsert : IDatabaseSessionInsert
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IApplicationLogger _appLogger;
        private readonly CodingSessionDTO _codingSessionDTO;



        public DatabaseSessionInsert(IDatabaseManager databaseManager, IApplicationLogger appLogger)
        {
            _databaseManager = databaseManager;
            _appLogger = appLogger;
        }


        public void InsertSession()
        {
            using (var activity = new Activity(nameof(InsertSession)).Start())
            {
                _appLogger.Debug($"Starting {nameof(InsertSession)}. TraceID: {activity.TraceId}, UserId: {_codingSessionDTO.UserId}");

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
                            StartDate,
                            EndDate,
                            DurationMinutes, 
                        ) 
                        VALUES 
                        (
                            @UserId, 
                            @StartTime, 
                            @EndTime, 
                            @StartDate,
                            @EndDate,
                            @DurationMinutes, 

                        )";

                    command.Parameters.AddWithValue("@UserId", _codingSessionDTO.UserId);
                    command.Parameters.AddWithValue("@StartTime", _codingSessionDTO.StartTime);
                    command.Parameters.AddWithValue("@EndTime", _codingSessionDTO.EndTime.HasValue ? (object)_codingSessionDTO.EndTime.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@StartDate", _codingSessionDTO.StartDate.HasValue ? _codingSessionDTO.StartDate.Value.ToString("yyyy-MM-dd") : DBNull.Value);
                    command.Parameters.AddWithValue("@EndDate", _codingSessionDTO.EndDate.HasValue ? _codingSessionDTO.EndDate.Value.ToString("yyyy-MM-dd") : DBNull.Value);
                    command.Parameters.AddWithValue("@DurationMinutes", _codingSessionDTO.DurationMinutes);


                    try
                    {
                        int affectedRows = command.ExecuteNonQuery();
                        stopwatch.Stop();
                        _appLogger.Info($"Session inserted successfully for UserId: {_codingSessionDTO.UserId}. Rows affected: {affectedRows}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }
                    catch (SQLiteException ex)
                    {
                        stopwatch.Stop();
                        _appLogger.Error($"Failed to insert session for UserId: {_codingSessionDTO.UserId}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                        Console.WriteLine("Failed to insert session");
                    }
                });
            }
        }
    }
}
