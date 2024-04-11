using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IDatabaseSessionReads;
using CodingTracker.Common.UserCredentialDTOs;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading.Tasks;
using CodingTracker.Common.IErrorHandlers;


// To do 
// Choose DTO properties what to pass a parameters


namespace CodingTracker.Data.DatabaseSessionReads
{
    public class DatabaseSessionRead : IDatabaseSessionRead
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IApplicationLogger _appLogger;
        private readonly CodingSessionDTO _codingSessionDTO;
        private readonly IErrorHandler _errorHandler;



        public DatabaseSessionRead(IDatabaseManager databaseManager, IApplicationLogger appLogger, IErrorHandler errorHandler)
        {
            _databaseManager = databaseManager;
            _appLogger = appLogger;
            _errorHandler = errorHandler;
        }


        public List<int> ReadSessionDurationMinutes(int numberOfDays, bool readAll = false)
        {
            using (var activity = new Activity(nameof(ReadSessionDurationMinutes)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ReadSessionDurationMinutes)}. TraceID: {activity.TraceId}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                List<int> durationMinutesList = new List<int>();

                try
                {
                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        string sqlQuery = @"
                    SELECT DurationMinutes FROM CodingSessions";


                        if (!readAll)
                        {
                            sqlQuery += " WHERE EndDate >= datetime('now', @DaysOffset)";
                        }

                        using var command = new SQLiteCommand(sqlQuery, connection);

                        // Set the parameter for the number of days offset if not reading all records
                        if (!readAll)
                        {
                            command.Parameters.AddWithValue("@DaysOffset", $"-{numberOfDays} days");
                        }

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
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

        public List<UserCredentialDTO> ReadUserCredentials(bool returnLastLoggedIn)
        {
            using (var activity = new Activity(nameof(ReadUserCredentials)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ReadUserCredentials)}. TraceID: {activity.TraceId}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                List<UserCredentialDTO> userCredentialsList = new List<UserCredentialDTO>();

                try
                {
                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using var command = connection.CreateCommand();

                        if (returnLastLoggedIn)
                        {
                            command.CommandText = @"
                        SELECT UserId, Username, PasswordHash, LastLogin
                        FROM UserCredentials 
                        ORDER BY LastLogin DESC 
                        LIMIT 1";
                        }
                        else
                        {
                            command.CommandText = @"
                        SELECT UserId, Username, PasswordHash, LastLogin
                        FROM UserCredentials";
                        }

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var userCredential = new UserCredentialDTO
                                {
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    Username = reader.GetString(reader.GetOrdinal("Username")),
                                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                    LastLogin = reader.GetDateTime(reader.GetOrdinal("LastLogin")) 
                                };
                                userCredentialsList.Add(userCredential);
                            }
                        }
                    });

                    stopwatch.Stop();
                    _appLogger.Info($"Successfully read user credentials. Count: {userCredentialsList.Count}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to read user credentials. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    throw;
                }

                return userCredentialsList;
            }
        }

        public int GetUserIdWithMostRecentLogin()
        {
            using (var activity = new Activity(nameof(GetUserIdWithMostRecentLogin)).Start())
            {
                _appLogger.Debug($"Starting {nameof(GetUserIdWithMostRecentLogin)}. TraceID: {activity.TraceId}");

                int userId = 0;
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using var command = new SQLiteCommand(@"
                            SELECT
                                UserId 
                            FROM
                                UserCredentials 
                            WHERE
                                LastLogin IS NOT NULL 
                            ORDER
                                BY LastLogin DESC 
                            LIMIT 
                                1",
                                    connection);

                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            userId = Convert.ToInt32(result);
                        }
                    });

                    stopwatch.Stop();
                    _appLogger.Info($"Successfully retrieved UserId with most recent login. UserId: {userId}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (SQLiteException ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($" Error retrieving UserId with most recent login. SQLite error code: {ex.ErrorCode}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($" Error retrieving UserId with most recent login. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    throw;
                }

                return userId;
            }
        }

        public int GetSessionIdWithMostRecentLogin()
        {
            using (var activity = new Activity(nameof(GetSessionIdWithMostRecentLogin)).Start())
            {
                _appLogger.Debug($"Starting {nameof(GetSessionIdWithMostRecentLogin)}. TraceID: {activity.TraceId}");

                int sessionId = 0;
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using var command = new SQLiteCommand(@"
                    SELECT
                        SessionId 
                    FROM
                        CodingSessions 
                    WHERE
                        EndTime IS NOT NULL 
                    ORDER BY
                        EndTime DESC 
                    LIMIT
                        1",
                            connection);

                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            sessionId = Convert.ToInt32(result);
                        }
                    });

                    stopwatch.Stop();
                    _appLogger.Info($"Successfully retrieved SessionId with most recent login. SessionId: {sessionId}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (SQLiteException ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($" Error retrieving SessionId with most recent login. SQLite error code: {ex.ErrorCode}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($" Error retrieving SessionId with most recent login. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    throw;
                }

                return sessionId;
            }
        }


        public List<CodingSessionDTO> ViewRecentSession(int numberOfSessions)
        {
            var methodName = nameof(ViewRecentSession);
            int userId = GetUserIdWithMostRecentLogin();
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();

            using (var activity = new Activity(nameof(ViewRecentSession)).Start())
            {
                _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}, NumberOfSessions: {numberOfSessions}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using var command = connection.CreateCommand();
                        command.CommandText = @"
                    SELECT SessionId, StartTime, EndTime FROM 
                        CodingSessions 
                    WHERE 
                        UserId = @UserId
                    ORDER BY 
                        StartDate DESC, StartTime DESC 
                    LIMIT 
                        @NumberOfSessions";

                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@NumberOfSessions", numberOfSessions);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var session = new CodingSessionDTO
                                {
                                    SessionId = reader.GetInt32(reader.GetOrdinal("SessionId")),
                                    StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                                    EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EndTime")),
                                };
                                codingSessionList.Add(session);
                            }
                        }
                    });

                    stopwatch.Stop();
                    _appLogger.Info($"Viewed recent {numberOfSessions} sessions successfully. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (SQLiteException ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to view recent sessions. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
            }
            return codingSessionList;
        }




        public List<CodingSessionDTO> ViewAllSession(bool partialView = false)
        {
            var methodName = nameof(ViewAllSession);
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();

            using (var activity = new Activity(nameof(ViewAllSession)).Start())
            {
                _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}, PartialView: {partialView}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using var command = connection.CreateCommand();
                        var partialColumns = partialView ? "SessionId, StartTime, EndTime" : "SessionId, StartTime, EndTime"; // Adjusted to match the columns used in the reader logic
                        command.CommandText = $@"
                    SELECT {partialColumns} FROM 
                        CodingSessions 
                    WHERE 
                        UserId = @UserId 
                    ORDER BY 
                        StartDate DESC, StartTime DESC";

                        command.Parameters.AddWithValue("@UserId", _codingSessionDTO.UserId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var session = new CodingSessionDTO
                                {
                                    SessionId = reader.GetInt32(reader.GetOrdinal("SessionId")),
                                    StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                                    EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EndTime")),
                                };
                                codingSessionList.Add(session);
                            }
                        }
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
            return codingSessionList;
        }

        public List<CodingSessionDTO> ViewSpecific(string chosenDate)
        {
            var methodName = nameof(ViewSpecific);
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();

            using (var activity = new Activity(methodName).Start())
            {
                _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}, ChosenDate: {chosenDate}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = @"
                        SELECT SessionId, StartTime, EndTime FROM 
                            CodingSessions 
                        WHERE
                            UserId = @UserId AND Date = @Date
                        ORDER BY
                            StartTime DESC";

                            command.Parameters.AddWithValue("@UserId", _codingSessionDTO.UserId);
                            command.Parameters.AddWithValue("@Date", chosenDate);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var session = new CodingSessionDTO
                                    {
                                        SessionId = reader.GetInt32(reader.GetOrdinal("SessionId")),
                                        StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                                        EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EndTime")),
                                    };
                                    codingSessionList.Add(session);
                                }
                            }
                        }
                    });

                    stopwatch.Stop();
                    _appLogger.Info($"{methodName} executed successfully. ChosenDate: {chosenDate}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}.");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to execute {methodName}. ChosenDate: {chosenDate}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
            }
            return codingSessionList;
        }

        public List<CodingSessionDTO> FilterSessionsByDay(string date, bool isDescending)
        {
            var methodName = nameof(FilterSessionsByDay);
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();

            using (var activity = new Activity(methodName).Start())
            {
                _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}, Date: {date}, IsDescending: {isDescending}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using var command = connection.CreateCommand();
                        string order = isDescending ? "DESC" : "ASC";
                        command.CommandText = $@"
                SELECT SessionId, StartTime, EndTime FROM
                    CodingSessions 
                WHERE
                    DATE(StartTime) = DATE(@Date)
                ORDER BY
                    StartTime {order}";

                        command.Parameters.AddWithValue("@Date", date);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var session = new CodingSessionDTO
                                {
                                    SessionId = reader.GetInt32(reader.GetOrdinal("SessionId")),
                                    StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                                    EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EndTime")),
                                };
                                codingSessionList.Add(session);
                            }
                        }
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
            return codingSessionList;
        }

        public List<CodingSessionDTO> FilterSessionsByWeek(string date, bool isDescending)
        {
            var methodName = nameof(FilterSessionsByWeek);
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();

            using (var activity = new Activity(nameof(FilterSessionsByWeek)).Start())
            {
                _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}, Date: {date}, IsDescending: {isDescending}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using var command = connection.CreateCommand();
                        string order = isDescending ? "DESC" : "ASC";
                        command.CommandText = $@"
                        SELECT SessionId, StartTime, EndTime FROM 
                            CodingSessions 
                        WHERE 
                            StartTime('%W', StartTime) = StartTime('%W', @Date) 
                        AND
                            StartTime('%Y', StartTime) = StartTime('%Y', @Date) 
                        AND 
                            DATE(StartTime) <= DATE(@Date)
                        ORDER BY StartTime {order}";

                        command.Parameters.AddWithValue("@Date", date);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var session = new CodingSessionDTO
                                {
                                    SessionId = reader.GetInt32(reader.GetOrdinal("SessionId")),
                                    StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                                    EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EndTime")),
                                };
                                codingSessionList.Add(session);
                            }
                        }
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
            return codingSessionList;
        }

        public List<CodingSessionDTO> FilterSessionsByYear(string year, bool isDescending)
        {
            var methodName = nameof(FilterSessionsByYear);
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();

            using (var activity = new Activity(nameof(FilterSessionsByYear)).Start())
            {
                _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}, Year: {year}, IsDescending: {isDescending}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using var command = connection.CreateCommand();
                        string order = isDescending ? "DESC" : "ASC";
                        string startDate = $"{year}-01-01";
                        string endDate = DateTime.Now.Year.ToString() == year ? DateTime.Now.ToString("yyyy-MM-dd") : $"{year}-12-31";
                        command.CommandText = $@"
                        SELECT SessionId, StartTime, EndTime FROM
                            CodingSessions 
                        WHERE
                            DATE(StartTime) >= DATE(@StartDate)
                        AND 
                            DATE(StartTime) <= DATE(@EndDate)
                        ORDER BY
                            StartTime {order}";

                        command.Parameters.AddWithValue("@StartDate", startDate);
                        command.Parameters.AddWithValue("@EndDate", endDate);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var session = new CodingSessionDTO
                                {
                                    SessionId = reader.GetInt32(reader.GetOrdinal("SessionId")),
                                    StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                                    EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EndTime")),
                                };
                                codingSessionList.Add(session);
                            }
                        }
                    });

                    stopwatch.Stop();
                    _appLogger.Info($"{methodName} executed successfully. Year: {year}, IsDescending: {isDescending}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to execute {methodName}. Year: {year}, IsDescending: {isDescending}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
            }
            return codingSessionList;
        }
    }
}
