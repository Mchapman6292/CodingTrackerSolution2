using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IDatabaseSessionReads;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading.Tasks;


namespace CodingTracker.Data.DatabaseSessionReads
{
    public class DatabaseSessionRead : IDatabaseSessionRead
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IApplicationLogger _appLogger;
        private readonly CodingSessionDTO _codingSessionDTO;



        public DatabaseSessionRead(IDatabaseManager databaseManager, IApplicationLogger appLogger)
        {
            _databaseManager = databaseManager;
            _appLogger = appLogger;
        }


        public async Task<List<int>> ReadSessionDurationMinutesAsync()
        {
            using (var activity = new Activity(nameof(ReadSessionDurationMinutesAsync)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ReadSessionDurationMinutesAsync)}. TraceID: {activity.TraceId}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                List<int> durationMinutesList = new List<int>();

                try
                {
                    await _databaseManager.ExecuteCRUDAsync(async connection =>
                    {
                        using var command = new SQLiteCommand(@"
                            SELECT DurationMinutes FROM CodingSessions", connection);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("DurationMinutes")))
                                {
                                    int durationMinutes = reader.GetInt32(reader.GetOrdinal("DurationMinutes"));
                                    durationMinutesList.Add(durationMinutes);
                                }
                            }
                        }
                    });

                    stopwatch.Stop();
                    _appLogger.Info($"Successfully read DurationMinutes values. Count: {durationMinutesList.Count}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to read DurationMinutes values. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    throw; 
                }

                return durationMinutesList;
            }
        }
        public async Task<List<CodingSessionDTO>> ViewRecentSession(int numberOfSessions)
        {
            var methodName = nameof(ViewRecentSession);
            using (var activity = new Activity(nameof(ViewRecentSession)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ViewRecentSession)}. TraceID: {activity.TraceId}, UserId: {_codingSessionDTO.UserId}, NumberOfSessions: {numberOfSessions}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                List<CodingSessionDTO> sessions = new List<CodingSessionDTO>();
                try 
                { 
                    await _databaseManager.ExecuteCRUDAsync(async connection =>
                    {
                        using var command = connection.CreateCommand();
                        command.CommandText = @"
                            SELECT * FROM
                                CodingSessions
                            WHERE 
                                UserId = @UserId
                            ORDER BY
                                StartDate DESC, StartTime DESC
                            LIMIT
                                @NumberOfSessions";

                        command.Parameters.AddWithValue("@UserId", _codingSessionDTO.UserId);
                        command.Parameters.AddWithValue("@NumberOfSessions", numberOfSessions);

                        await command.ExecuteReaderAsync();
                    });

                    stopwatch.Stop();
                    _appLogger.Info($"Viewed recent {numberOfSessions} sessions successfully. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }
                catch (SQLiteException ex)
                    {
                        stopwatch.Stop();
                        _appLogger.Error($"Failed to view recent sessions. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);

                    }
                    }
                  
                }
            

        public async Task ViewAllSession(bool partialView = false)
        {
            var methodName = nameof(ViewAllSession);
            using (var activity = new Activity(methodName).Start())
            {
                _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}, PartialView: {partialView}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    await _databaseManager.ExecuteCRUDAsync(async connection =>
                    {
                        using var command = connection.CreateCommand();
                        var partialColumns = partialView ? "SessionId, StartDate, EndDate" : "*";
                        command.CommandText = $@"
                    SELECT {partialColumns} FROM
                        CodingSessions
                    WHERE
                        UserId = @UserId
                    ORDER BY
                        StartDate DESC, StartTime DESC";

                        command.Parameters.AddWithValue("@UserId", _codingSessionDTO.UserId);

                        await command.ExecuteReaderAsync(); 
                    });

                    stopwatch.Stop();
                    _appLogger.Info($"{methodName} executed successfully. PartialView: {partialView}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to execute {methodName}. PartialView: {partialView}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
            }
        }

        public async Task ViewSpecific(string chosenDate)
        {
            var methodName = nameof(ViewSpecific);
            using (var activity = new Activity(methodName).Start())
            {
                _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}, ChosenDate: {chosenDate}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    await _databaseManager.ExecuteCRUDAsync(async connection =>
                    {
                        using var command = connection.CreateCommand();
                        command.CommandText = @"
                    SELECT SessionId, StartTime, EndTime FROM 
                        CodingSessions 
                    WHERE
                        UserId = @UserId AND Date = @Date
                    ORDER BY
                        StartTime DESC";

                        command.Parameters.AddWithValue("@UserId", _codingSessionDTO.UserId);
                        command.Parameters.AddWithValue("@Date", chosenDate);

                        await command.ExecuteReaderAsync();

                        stopwatch.Stop();
                        _appLogger.Info($"{methodName} executed successfully. ChosenDate: {chosenDate}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
          
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to execute {methodName}. ChosenDate: {chosenDate}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
            }
        }

        public async Task FilterSessionsByDay(string date, bool isDescending)
        {
            var methodName = nameof(FilterSessionsByDay);
            using (var activity = new Activity(methodName).Start())
            {
                _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}, Date: {date}, IsDescending: {isDescending}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    await _databaseManager.ExecuteCRUDAsync(async connection =>
                    {
                        using var command = connection.CreateCommand();
                        string order = isDescending ? "DESC" : "ASC";
                        command.CommandText = $@"
                    SELECT * FROM
                        CodingSessions 
                    WHERE
                        DATE(StartTime) = DATE(@Date)
                    ORDER BY
                        StartTime {order}";

                        command.Parameters.AddWithValue("@Date", date);

                        await command.ExecuteReaderAsync();
                    });

                    stopwatch.Stop();
                    _appLogger.Info($"{methodName} executed successfully. Date: {date}, IsDescending: {isDescending}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to execute {methodName}. Date: {date}, IsDescending: {isDescending}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
            }
        }

        public async Task FilterSessionsByWeek(string date, bool isDescending)
        {
            var methodName = nameof(FilterSessionsByWeek);
            using (var activity = new Activity(methodName).Start())
            {
                _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}, Date: {date}, IsDescending: {isDescending}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    await _databaseManager.ExecuteCRUDAsync(async connection =>
                    {
                        using var command = new SQLiteCommand(connection);
                        string order = isDescending ? "DESC" : "ASC";
                        command.CommandText = $@"
                    SELECT * FROM 
                        CodingSessions 
                    WHERE strftime('%W', StartTime) = strftime('%W', @Date) AND 
                          strftime('%Y', StartTime) = strftime('%Y', @Date) 
                    ORDER BY StartTime {order}";

                        command.Parameters.AddWithValue("@Date", date);

                        await command.ExecuteReaderAsync();
                    });

                    stopwatch.Stop();
                    _appLogger.Info($"{methodName} executed successfully. Date: {date}, IsDescending: {isDescending}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (SQLiteException ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to execute {methodName}. Date: {date}, IsDescending: {isDescending}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
            }
        }

        public async Task FilterSessionsByYear(string year, bool isDescending)
        {
            var methodName = nameof(FilterSessionsByYear);
            using (var activity = new Activity(methodName).Start())
            {
                _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}, Year: {year}, IsDescending: {isDescending}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    await _databaseManager.ExecuteCRUDAsync(async connection =>
                    {
                        using var command = new SQLiteCommand(connection);
                        string order = isDescending ? "DESC" : "ASC";
                        command.CommandText = $@"
                    SELECT * FROM
                        CodingSessions 
                    WHERE
                        strftime('%Y', StartTime) = @Year 
                    ORDER BY
                        StartTime {order}";

                        command.Parameters.AddWithValue("@Year", year);

                        await command.ExecuteReaderAsync(); 
                    });

                    stopwatch.Stop();
                    _appLogger.Info($"{methodName} executed successfully. Year: {year}, IsDescending: {isDescending}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (SQLiteException ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to execute {methodName}. Year: {year}, IsDescending: {isDescending}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
            }
        }
    }
}
