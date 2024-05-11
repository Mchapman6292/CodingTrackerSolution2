using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IDatabaseSessionReads;
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.ICredentialManagers;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading.Tasks;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.UserCredentialDTOManagers;
using CodingTracker.Common.IQueryBuilders;


// To do 
// Choose DTO properties what to pass a parameters


namespace CodingTracker.Data.DatabaseSessionReads
{
    public class DatabaseSessionRead : IDatabaseSessionRead
    {
        private readonly CodingSessionDTO _codingSessionDTO;
        private readonly UserCredentialDTO _currentUserCredentialDTO;

        private readonly IDatabaseManager _databaseManager;
        private readonly IApplicationLogger _appLogger;
        private readonly IErrorHandler _errorHandler;
        private readonly ICredentialManager _credentialManager;
        private readonly IQueryBuilder _queryBuilder;

        private int _currentUserId;




        public DatabaseSessionRead(IDatabaseManager databaseManager, IApplicationLogger appLogger, IErrorHandler errorHandler, ICredentialManager credentialManager, IQueryBuilder queryBuilder)
        {
            _databaseManager = databaseManager;
            _appLogger = appLogger;
            _errorHandler = errorHandler;
            _credentialManager = credentialManager;
            _queryBuilder = queryBuilder;
        }




        public List<double> ReadSessionDurationSeconds(int numberOfDays, bool readAll = false)
        {
            List<double> durationSecondsList = new List<double>();
            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(connection);

                if (readAll)
                {
                    command.CommandText = @"
                    SELECT 
                            durationSeconds
                    FROM
                            CodingSessions";
                }
                else
                {
                    command.CommandText = @"
                    SELECT
                            durationSeconds 
                    FROM
                            CodingSessions
                    WHERE
                            startTime >= datetime('now', @DaysOffset)
                    AND
                            endTime <= datetime('now')";
                    command.Parameters.AddWithValue("@DaysOffset", $"-{numberOfDays} days");
                }
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(reader.GetOrdinal("durationSeconds")))
                        {
                            _appLogger.Error($"No durationSeconds values found for ReadSessionDurationSeconds.");
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("durationSeconds")))
                        {
                            double durationSeconds = reader.GetDouble(reader.GetOrdinal("durationSeconds"));
                            durationSecondsList.Add(durationSeconds);
                        }
                    }
                }
            }, nameof(ReadSessionDurationSeconds));
            return durationSecondsList;
        }

        public List<UserCredentialDTO> ReadUserCredentials(bool returnLastLoggedIn)
        {
            List<UserCredentialDTO> userCredentialsList = new List<UserCredentialDTO>();
            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = connection.CreateCommand();

                if (returnLastLoggedIn)
                {
                    command.CommandText = @"
                    SELECT 
                            userId,
                            Username,
                            PasswordHash,
                            LastLogin
                    FROM
                            UserCredentials 
                    ORDER BY
                            LastLogin DESC 
                    LIMIT 1";
                }
                else
                {
                    command.CommandText = @"
                          SELECT 
                                  userId, 
                                  Username,
                                  PasswordHash, 
                                  LastLogin
                          FROM 
                                  UserCredentials";
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var userCredential = new UserCredentialDTO
                        {
                            UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                            LastLogin = reader.GetDateTime(reader.GetOrdinal("LastLogin"))
                        };
                        userCredentialsList.Add(userCredential);
                    }
                }
            }, nameof(ReadUserCredentials));
            return userCredentialsList;
        }




        public List<CodingSessionDTO> ViewAllSession(bool partialView = false)
        {
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
                    SELECT
                            sessionId,
                            goalHHMM,
                            durationHHMM,
                            startTime, 
                            endTime
                    FROM
                            CodingSessions
                    WHERE
                            userId = @userId 
                    ORDER BY 
                            startTime DESC",

                            connection);

                command.Parameters.AddWithValue("@userId", _codingSessionDTO.userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var session = new CodingSessionDTO
                        {
                            sessionId = reader.GetInt32(reader.GetOrdinal("sessionId")),
                            goalHHMM = reader.IsDBNull(reader.GetOrdinal("goalHHMM")) ? null : reader.GetString(reader.GetOrdinal("goalHHMM")),
                            durationHHMM = reader.IsDBNull(reader.GetOrdinal("durationHHMM")) ? null : reader.GetString(reader.GetOrdinal("durationHHMM")),
                            startTime = reader.GetDateTime(reader.GetOrdinal("startTime")),
                            endTime = reader.IsDBNull(reader.GetOrdinal("endTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("endTime"))
                        };
                        codingSessionList.Add(session);
                    }
                }
            }, nameof(ViewAllSession));

            return codingSessionList;
        }


        public List<CodingSessionDTO> ViewSpecific(DateTime chosenDate)
        {
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();

            int userId = ReturnCurrentUserId();

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
                    SELECT
                            userId,
                            sessionId, 
                            startTime, 
                            endTime,
                            durationSeconds REAL,
                            durationHHMM STRING,
                            goalHHMM STRING
                    FROM
                            CodingSessions
                    WHERE 
                            userId = @userId 
                    AND 
                            startTime = @Date
                ORDER BY
                            startTime DESC",

                            connection);

                command.Parameters.AddWithValue("@userId", userId); // Needs to be taken from usercredential 
                command.Parameters.AddWithValue("@Date", chosenDate);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var session = new CodingSessionDTO
                        {
                            userId = reader.GetInt32(reader.GetOrdinal("userId")),
                            sessionId = reader.GetInt32(reader.GetOrdinal("sessionId")),
                            startTime = reader.GetDateTime(reader.GetOrdinal("startTime")),
                            endTime = reader.IsDBNull(reader.GetOrdinal("endTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("endTime")),
                            durationSeconds = reader.IsDBNull(reader.GetOrdinal("durationSeconds")) ? (double?)null : reader.GetDouble(reader.GetOrdinal("durationSeconds")),
                            durationHHMM = reader.IsDBNull(reader.GetOrdinal("durationHHMM")) ? null : reader.GetString(reader.GetOrdinal("durationHHMM")),
                            goalHHMM = reader.IsDBNull(reader.GetOrdinal("goalHHMM")) ? null : reader.GetString(reader.GetOrdinal("goalHHMM"))
                        };
                        codingSessionList.Add(session);
                    }
                }
            }, nameof(ViewSpecific));

            return codingSessionList;
        }



        public List<CodingSessionDTO> ViewRecentSession(int numberOfSessions)
        {
            int userId = GetUserIdWithMostRecentLogin();
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
                    SELECT
                            sessionId,
                            startTime,
                            endTime,
                            durationSeconds,
                            durationHHMM,
                            goalHHMM

                    FROM
                            CodingSessions 
                    WHERE
                            userId = @userId
                ORDER BY
                            startTime DESC 
                    LIMIT
                            @NumberOfSessions", connection);

                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@NumberOfSessions", numberOfSessions);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var session = new CodingSessionDTO
                        {
                            sessionId = reader.GetInt32(reader.GetOrdinal("sessionId")),
                            startTime = reader.GetDateTime(reader.GetOrdinal("startTime")),
                            endTime = reader.IsDBNull(reader.GetOrdinal("endTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("endTime")),
                            durationSeconds = reader.IsDBNull(reader.GetOrdinal("durationSeconds")) ? (double?)null : reader.GetDouble(reader.GetOrdinal("durationSeconds")),
                            durationHHMM = reader.IsDBNull(reader.GetOrdinal("durationHHMM")) ? null : reader.GetString(reader.GetOrdinal("durationHHMM")),
                            goalHHMM = reader.IsDBNull(reader.GetOrdinal("goalHHMM")) ? null : reader.GetString(reader.GetOrdinal("goalHHMM"))
                        };
                        codingSessionList.Add(session);
                    }
                }
            }, nameof(ViewRecentSession));

            return codingSessionList;
        }





        public List<CodingSessionDTO> SelectAllSessionsForDate(string date, bool isDescending)
        {
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();
            string order = isDescending ? "DESC" : "ASC";

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
                    SELECT
                            sessionId, 
                            startTime, 
                            endTime,
                            durationSeconds
                    FROM 
                            CodingSessions 
                    WHERE 
                            DATE(startTime) = DATE(@Date)
                ORDER BY
                            startTime " + order, connection);

                command.Parameters.AddWithValue("@Date", date);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        codingSessionList.Add(new CodingSessionDTO
                        {
                            sessionId = reader.GetInt32(reader.GetOrdinal("sessionId")),
                            startTime = reader.GetDateTime(reader.GetOrdinal("startTime")),
                            endTime = reader.IsDBNull(reader.GetOrdinal("endTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("endTime")),
                            durationSeconds = reader.IsDBNull(reader.GetOrdinal("durationSeconds")) ? (double?)null : reader.GetDouble(reader.GetOrdinal("durationSeconds"))
                        });
                    }
                }
            }, nameof(SelectAllSessionsForDate));

            return codingSessionList;
        }

        public List<CodingSessionDTO> SelectAllSesssionsForWeek(string date, bool isDescending)
        {
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();
            string order = isDescending ? "DESC" : "ASC";

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
                    SELECT
                            sessionId, 
                            startTime, 
                            endTime 
                    FROM 
                            CodingSessions 
                    WHERE
                            strftime('%W', startTime) = strftime('%W', @Date) 
                    AND 
                            strftime('%Y', startTime) = strftime('%Y', @Date)
                ORDER BY
                            startTime " + order, connection);

                command.Parameters.AddWithValue("@Date", date);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        codingSessionList.Add(new CodingSessionDTO
                        {
                            sessionId = reader.GetInt32(reader.GetOrdinal("sessionId")),
                            startTime = reader.GetDateTime(reader.GetOrdinal("startTime")),
                            endTime = reader.IsDBNull(reader.GetOrdinal("endTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("endTime"))
                        });
                    }
                }
            }, nameof(SelectAllSesssionsForWeek));

            return codingSessionList;
        }



        public List<CodingSessionDTO> SelectAllSessionsForYear(string year, bool isDescending)
        {
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();
            string order = isDescending ? "DESC" : "ASC";
            string yearStartTime = $"{year}-01-01";
            string yearEndTime = DateTime.Now.Year.ToString() == year ? DateTime.Now.ToString("yyyy-MM-dd") : $"{year}-12-31";

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
                    SELECT 
                            sessionId, 
                            startTime, 
                            endTime 
                    FROM 
                            CodingSessions 
                    WHERE
                            DATE(startTime) >= DATE(@YearStartTime)
                    AND
                            DATE(startTime) <= DATE(@YearEndTime)
                ORDER BY
                            startTime "

                            + order, connection);

                command.Parameters.AddWithValue("@YearStartTime", yearStartTime);
                command.Parameters.AddWithValue("@YearEndTime", yearEndTime);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        codingSessionList.Add(new CodingSessionDTO
                        {
                            sessionId = reader.GetInt32(reader.GetOrdinal("sessionId")),
                            startTime = reader.GetDateTime(reader.GetOrdinal("startTime")),
                            endTime = reader.IsDBNull(reader.GetOrdinal("endTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("endTime"))
                        });
                    }
                }
            }, nameof(SelectAllSessionsForYear));

            return codingSessionList;
        }


        public void GetLast28DaysSessions()
        {
            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
                    SELECT 
                            startTime, 
                            durationSeconds
                    FROM 
                            CodingSessions
                    WHERE 
                            startTime >= @startTime
                    AND 
                            startTime <= @endTime",
                    connection);

                var endTime = DateTime.UtcNow.Date;
                var startTime = endTime.AddDays(-28);

                command.Parameters.AddWithValue("@startTime", startTime);
                command.Parameters.AddWithValue("@endTime", endTime);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var durationSeconds = reader["durationSeconds"];
                        var sessionTime = reader["startTime"];
                        _appLogger.Info($"Session at {sessionTime} - Duration: {durationSeconds} seconds.");
                    }
                }
            }, nameof(GetLast28DaysSessions));
        }



        public List<(DateTime Date, double TotalDurationSeconds)> ReadDurationSecondsLast28Days()
        {
            List<(DateTime Date, double TotalDurationSeconds)> dailyDurations = new List<(DateTime Date, double TotalDurationSeconds)>();
            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(connection);

                command.CommandText = @"
                    SELECT 
                        date(startTime) AS SessionDay,
                        SUM(durationSeconds) AS TotalDurationSeconds
                    FROM
                        CodingSessions
                    WHERE
                        date(startTime) BETWEEN date('now', '-29 days') AND date('now', '-1 day')
                    GROUP BY
                        SessionDay
                    ORDER BY
                        SessionDay DESC";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime sessionDay = reader.GetDateTime(reader.GetOrdinal("SessionDay"));
                        double totalDurationSeconds = reader.GetDouble(reader.GetOrdinal("TotalDurationSeconds"));
                        dailyDurations.Add((sessionDay, totalDurationSeconds));
                    }
                }
            }, nameof(ReadDurationSecondsLast28Days));

            _appLogger.Info($" ReadDurationSecondsLast28Days startDate: {DateTime.Now.AddDays(-29):yyyy-MM-dd}, endDate: {DateTime.Now.AddDays(-1):yyyy-MM-dd}.");
            return dailyDurations;
        }

        public int GetSessionIdWithMostRecentLogin()
        {
            using (var activity = new Activity(nameof(GetSessionIdWithMostRecentLogin)).Start())
            {
                _appLogger.Debug($"Starting {nameof(GetSessionIdWithMostRecentLogin)}. TraceID: {activity.TraceId}");
                Stopwatch stopwatch = Stopwatch.StartNew();
                int sessionId = 0;

                try
                {
                    _databaseManager.ExecuteDatabaseOperation(connection =>
                    {
                        using var command = new SQLiteCommand(@"
                SELECT
                        sessionId
                FROM
                        CodingSessions
                WHERE 
                        endTime IS NOT NULL
                ORDER BY
                        endTime DESC
                LIMIT 1",
                        connection);

                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            sessionId = Convert.ToInt32(result);
                        }
                    }, nameof(GetSessionIdWithMostRecentLogin));
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"An error occurred during {nameof(GetSessionIdWithMostRecentLogin)}. Error: {ex.Message}. TraceID: {activity.TraceId}, Elapsed time: {stopwatch.ElapsedMilliseconds}ms.", ex);
                    throw;
                }

                stopwatch.Stop();
                _appLogger.Info($"{nameof(GetSessionIdWithMostRecentLogin)} completed. Retrieved sessionId: {sessionId}. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                return sessionId;
            }
        }



        public int GetUserIdWithMostRecentLogin()
        {
            using (var activity = new Activity(nameof(GetUserIdWithMostRecentLogin)).Start())
            {
                _appLogger.Debug($"Starting {nameof(GetUserIdWithMostRecentLogin)}. TraceID: {activity.TraceId}");
                Stopwatch stopwatch = Stopwatch.StartNew();
                int userId = 0;

                try
                {
                    _databaseManager.ExecuteDatabaseOperation(connection =>
                    {
                        using var command = new SQLiteCommand(@"
                SELECT 
                        userId
                FROM
                        UserCredentials
                WHERE
                        LastLogin IS NOT NULL
                ORDER BY
                        LastLogin DESC
                LIMIT
                        1",
                        connection);

                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            userId = Convert.ToInt32(result);
                        }
                    }, nameof(GetUserIdWithMostRecentLogin));
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"An error occurred during {nameof(GetUserIdWithMostRecentLogin)}. Error: {ex.Message}. TraceID: {activity.TraceId}, Elapsed time: {stopwatch.ElapsedMilliseconds}ms.", ex);
                    throw;
                }

                stopwatch.Stop();
                _appLogger.Info($"{nameof(GetUserIdWithMostRecentLogin)} completed. Retrieved userId: {userId}. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                return userId;
            }
        }




        public UserCredentialDTO ValidateLogin(string username, string password)
        {
            var lastLogin = string.Empty;
            using (var activity = new Activity(nameof(ValidateLogin)).Start())
            {
                _appLogger.Info($"Starting {nameof(ValidateLogin)}. TraceID: {activity.TraceId}, Username: {username}");
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    _appLogger.Debug($"Hashing password for {username}. TraceID: {activity.TraceId}");
                    var hashedPassword = _credentialManager.HashPassword(password);
                    _appLogger.Debug($"PasswordHash hashed for {username}. TraceID: {activity.TraceId}");

                    UserCredentialDTO userCredential = null;

                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using var command = connection.CreateCommand();
                        command.CommandText = @"
                                SELECT 
                                        userId, 
                                        Username, 
                                        PasswordHash,
                                        LastLogin
                                FROM 
                                        UserCredentials
                                WHERE 
                                        Username = @Username";

                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@PasswordHash", password);
                        command.Parameters.AddWithValue("@LastLogin", lastLogin);

                        _appLogger.Debug($"Executing database query for {username}. TraceID: {activity.TraceId}");


                        using var reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            _appLogger.Debug($"User record found for {username}. TraceID: {activity.TraceId}");

                            var storedHash = reader["PasswordHash"].ToString();
                            if (hashedPassword == storedHash)
                            {
                                _appLogger.Debug($"PasswordHash match for {username}. TraceID: {activity.TraceId}");

                                userCredential = new UserCredentialDTO
                                {
                                    UserId = Convert.ToInt32(reader["userId"]),
                                    Username = reader["Username"].ToString(),
                                    PasswordHash = password
                                };
                            }
                            else
                            {
                                _appLogger.Info($"PasswordHash mismatch for {username}. TraceID: {activity.TraceId}");
                            }
                        }
                        else
                        {
                            _appLogger.Info($"No user record found for {username}. TraceID: {activity.TraceId}");
                        }
                    });

                    stopwatch.Stop();
                    if (userCredential != null)
                    {
                        _appLogger.Info($"User {username} validated successfully. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                        AssignCurrentUserId(userCredential.UserId);
                        _appLogger.Info($" userId created in NewUserCredentialDTO {userCredential.UserId} assigned to _currentUserId property {_currentUserId}.");
                    }
                    else
                    {
                        _appLogger.Info($"User {username} validation failed. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }

                    return userCredential; // Returns null if no matching user 
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(ValidateLogin)} for username {username}. Error: {ex.GetType().Name} - {ex.Message}. TraceID: {activity.TraceId}", ex);
                    return null;
                }
            }
        }




        public void AssignCurrentUserId(int? userId)
        {
            using (var activity = new Activity(nameof(ReturnCurrentUserId)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ReturnCurrentUserId)}. TraceID: {activity.TraceId}.");

                _currentUserId = (int)userId;
            }
        }

        public int ReturnCurrentUserId()
        {
            using (var activity = new Activity(nameof(ReturnCurrentUserId)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ReturnCurrentUserId)}. TraceID: {activity.TraceId}.");

                if (_currentUserId == 0)
                {
                    _appLogger.Info($"CurrentUserId is 0 (default for not created.");
                }
                else
                {
                    return _currentUserId;
                }
                return _currentUserId;
            }
        }



     
    }
}
