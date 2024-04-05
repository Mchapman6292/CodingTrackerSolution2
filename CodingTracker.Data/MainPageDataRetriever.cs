using System;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IApplicationLoggers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.CodingSessionDTOs;
using System.Data.SQLite;
using System.Diagnostics;

namespace CodingTracker.Data.MainPageDataRetrievers
{
    public class MainPageDataRetriever
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IApplicationLogger _appLogger;

        public MainPageDataRetriever(IDatabaseManager databaseManager, IApplicationLogger appLogger)
        {
            _databaseManager = databaseManager;
            _appLogger = appLogger;
        }


        public void GetLast28DaysSessions()
        {
            using (var activity = new Activity(nameof(GetLast28DaysSessions)).Start())
            {
                _appLogger.Info($"Starting {nameof(GetLast28DaysSessions)}. TraceID: {activity.TraceId}");
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using var command = connection.CreateCommand();
                        var endDate = DateTime.UtcNow.Date;
                        var startDate = endDate.AddDays(-28);

                        command.CommandText = @"
                    SELECT 
                        DurationMinutes
                    FROM 
                        CodingSessions
                    WHERE 
                        StartDate >= @StartDate AND EndDate <= @EndDate";

                        command.Parameters.AddWithValue("@StartDate", startDate);
                        command.Parameters.AddWithValue("@EndDate", endDate);

                        using var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            _appLogger.Info($"DurationMinutes: {reader["DurationMinutes"]}, Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                        }
                    });

                    stopwatch.Stop();
                    _appLogger.Info($"{nameof(GetLast28DaysSessions)}  Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during{nameof(GetLast28DaysSessions)}  Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }
    }
}
