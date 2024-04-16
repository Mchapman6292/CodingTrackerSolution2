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
        private readonly ICredentialManager _credentialManager;



        public DatabaseSessionRead(IDatabaseManager databaseManager, IApplicationLogger appLogger, IErrorHandler errorHandler, ICredentialManager credentialManager)
        {
            _databaseManager = databaseManager;
            _appLogger = appLogger;
            _errorHandler = errorHandler;
            _credentialManager = credentialManager;
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
                            DurationSeconds
                    FROM
                            CodingSessions";
                }
                else
                {
                    command.CommandText = @"
                    SELECT
                            DurationSeconds 
                    FROM
                            CodingSessions
                    WHERE
                            StartDate >= datetime('now', @DaysOffset)
                    AND
                            EndDate <= datetime('now')";
                    command.Parameters.AddWithValue("@DaysOffset", $"-{numberOfDays} days");
                }
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("DurationSeconds")))
                        {
                            double durationSeconds = reader.GetInt32(reader.GetOrdinal("DurationSeconds"));
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
                            UserId,
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
                                  UserId, 
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
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
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


        public List<(DateTime Day, double TotalDurationSeconds)> ReadTotalSessionDurationByDay()
        {
            List<(DateTime Day, double TotalDurationSeconds)> dailyDurations = new List<(DateTime Day, double TotalDurationSeconds)>();
            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(connection);

                command.CommandText = @"
                    SELECT 
                        date(StartTime) AS SessionDay,
                        SUM(DurationSeconds) AS TotalDurationSeconds
                    FROM
                        CodingSessions
                    WHERE
                        date(StartTime) BETWEEN date('now', '-29 days') AND date('now', '-1 day')
                    GROUP BY
                        SessionDay
                    ORDER BY
                        SessionDay DESC";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime sessionDay = reader.GetDateTime(reader.GetOrdinal("SessionDay"));
                        double totalDurationSeconds = reader.GetInt32(reader.GetOrdinal("TotalDurationSeconds"));
                        dailyDurations.Add((sessionDay, totalDurationSeconds));
                    }
                }
            }, nameof(ReadTotalSessionDurationByDay));

            return dailyDurations;
        }


        public int GetSessionIdWithMostRecentLogin()
        {
            int sessionId = 0;
            _databaseManager.ExecuteDatabaseOperation(connection =>
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
                    LIMIT 1",
                    
                    connection);

                object result = command.ExecuteScalar(); 
                if (result != null && result != DBNull.Value)
                {
                    sessionId = Convert.ToInt32(result);
                }
            }, nameof(GetSessionIdWithMostRecentLogin));

            return sessionId;
        }

        public List<CodingSessionDTO> ViewRecentSession(int numberOfSessions)
        {
            int userId = _credentialManager.GetUserIdWithMostRecentLogin();
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
                    SELECT
                            SessionId,
                            StartTime,
                            EndTime,
                            DurationSeconds,
                            DurationHHMM,
                            GoalHHMM

                    FROM
                            CodingSessions 
                    WHERE
                            UserId = @UserId
                ORDER BY
                            StartTime DESC 
                    LIMIT
                            @NumberOfSessions", connection);

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
                            DurationSeconds = reader.IsDBNull(reader.GetOrdinal("DurationSeconds")) ? (double?)null : reader.GetDouble(reader.GetOrdinal("DurationSeconds")),
                            DurationHHMM = reader.IsDBNull(reader.GetOrdinal("DurationHHMM")) ? null : reader.GetString(reader.GetOrdinal("DurationHHMM")),
                            GoalHHMM = reader.IsDBNull(reader.GetOrdinal("GoalHHMM")) ? null : reader.GetString(reader.GetOrdinal("GoalHHMM"))
                        };
                        codingSessionList.Add(session);
                    }
                }
            }, nameof(ViewRecentSession));

            return codingSessionList;
        }




        public List<CodingSessionDTO> ViewAllSession(bool partialView = false)
        {
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
            SELECT
                    SessionId,
                    GoalHHMM,
                    DurationHHMM,
                    StartTime, 
                    EndTime
            FROM
                    CodingSessions
            WHERE
                    UserId = @UserId 
            ORDER BY 
                    StartTime DESC",

                            connection);

                command.Parameters.AddWithValue("@UserId", _codingSessionDTO.UserId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var session = new CodingSessionDTO
                        {
                            SessionId = reader.GetInt32(reader.GetOrdinal("SessionId")),
                            GoalHHMM = reader.IsDBNull(reader.GetOrdinal("GoalHHMM")) ? null : reader.GetString(reader.GetOrdinal("GoalHHMM")),
                            DurationHHMM = reader.IsDBNull(reader.GetOrdinal("DurationHHMM")) ? null : reader.GetString(reader.GetOrdinal("DurationHHMM")),
                            StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                            EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EndTime"))
                        };
                        codingSessionList.Add(session);
                    }
                }
            }, nameof(ViewAllSession));

            return codingSessionList;
        }


        public List<CodingSessionDTO> ViewSpecific(string chosenDate)
        {
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
                    SELECT
                            SessionId, 
                            StartTime, 
                            EndTime 
                    FROM 
                            CodingSessions 
                    WHERE 
                            UserId = @UserId 
                    AND 
                            Date = @Date
                ORDER BY
                            StartTime DESC", 
                            
                            connection);

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
            }, nameof(ViewSpecific));

            return codingSessionList;
        }

        public List<CodingSessionDTO> FilterSessionsByDay(string date, bool isDescending)
        {
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();
            string order = isDescending ? "DESC" : "ASC";

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
                    SELECT
                            SessionId, 
                            StartTime, 
                            EndTime 
                    FROM 
                            CodingSessions 
                    WHERE 
                            DATE(StartTime) = DATE(@Date)
                ORDER BY
                            StartTime " + order, connection);

                command.Parameters.AddWithValue("@Date", date);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        codingSessionList.Add(new CodingSessionDTO
                        {
                            SessionId = reader.GetInt32(reader.GetOrdinal("SessionId")),
                            StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                            EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EndTime"))
                        });
                    }
                }
            }, nameof(FilterSessionsByDay));

            return codingSessionList;
        }

        public List<CodingSessionDTO> FilterSessionsByWeek(string date, bool isDescending)
        {
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();
            string order = isDescending ? "DESC" : "ASC";

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
                    SELECT
                            SessionId, 
                            StartTime, 
                            EndTime 
                    FROM 
                            CodingSessions 
                    WHERE
                            strftime('%W', StartTime) = strftime('%W', @Date) 
                    AND 
                            strftime('%Y', StartTime) = strftime('%Y', @Date)
                ORDER BY
                            StartTime " + order, connection);

                command.Parameters.AddWithValue("@Date", date);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        codingSessionList.Add(new CodingSessionDTO
                        {
                            SessionId = reader.GetInt32(reader.GetOrdinal("SessionId")),
                            StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                            EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EndTime"))
                        });
                    }
                }
            }, nameof(FilterSessionsByWeek));

            return codingSessionList;
        }



        public List<CodingSessionDTO> FilterSessionsByYear(string year, bool isDescending)
        {
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();
            string order = isDescending ? "DESC" : "ASC";
            string yearStartTime = $"{year}-01-01";
            string yearEndTime = DateTime.Now.Year.ToString() == year ? DateTime.Now.ToString("yyyy-MM-dd") : $"{year}-12-31";

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
                    SELECT 
                            SessionId, 
                            StartTime, 
                            EndTime 
                    FROM 
                            CodingSessions 
                    WHERE
                            DATE(StartTime) >= DATE(@YearStartTime)
                    AND
                            DATE(StartTime) <= DATE(@YearEndTime)
                ORDER BY
                            StartTime " 

                            + order, connection);

                command.Parameters.AddWithValue("@YearStartTime", yearStartTime);
                command.Parameters.AddWithValue("@YearEndTime", yearEndTime);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        codingSessionList.Add(new CodingSessionDTO
                        {
                            SessionId = reader.GetInt32(reader.GetOrdinal("SessionId")),
                            StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                            EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EndTime"))
                        });
                    }
                }
            }, nameof(FilterSessionsByYear));

            return codingSessionList;
        }


        public void GetLast28DaysSessions()
        {
            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
                    SELECT 
                            StartTime, 
                            DurationSeconds
                    FROM 
                            CodingSessions
                    WHERE 
                            StartTime >= @StartTime
                    AND 
                            StartTime <= @EndTime",
                    connection);

                var endTime = DateTime.UtcNow.Date;
                var startTime = endTime.AddDays(-28);

                command.Parameters.AddWithValue("@StartTime", startTime);
                command.Parameters.AddWithValue("@EndTime", endTime);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var durationSeconds = reader["DurationSeconds"];
                        var sessionTime = reader["StartTime"];
                        _appLogger.Info($"Session at {sessionTime} - Duration: {durationSeconds} seconds.");
                    }
                }
            }, nameof(GetLast28DaysSessions));
        }
    }
}
