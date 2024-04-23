﻿using CodingTracker.Common.CodingSessionDTOs;
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
                            StartTime >= datetime('now', @DaysOffset)
                    AND
                            EndTime <= datetime('now')";
                    command.Parameters.AddWithValue("@DaysOffset", $"-{numberOfDays} days");
                }
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(reader.GetOrdinal("DurationSeconds")))
                        {
                            _appLogger.Error($"No DurationSeconds values found for ReadSessionDurationSeconds.");
                        }
                       if (!reader.IsDBNull(reader.GetOrdinal("DurationSeconds")))
                        {
                            double durationSeconds = reader.GetDouble(reader.GetOrdinal("DurationSeconds"));
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


        public List<CodingSessionDTO> ViewSpecific(DateTime chosenDate)
        {
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
                    SELECT
                            UserId,
                            SessionId, 
                            StartTime, 
                            EndTime,
                            DurationSeconds REAL,
                            DurationHHMM STRING,
                            GoalHHMM STRING
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
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
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





        public List<CodingSessionDTO> SelectAllSessionsForDate(string date, bool isDescending)
        {
            List<CodingSessionDTO> codingSessionList = new List<CodingSessionDTO>();
            string order = isDescending ? "DESC" : "ASC";

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
                    SELECT
                            SessionId, 
                            StartTime, 
                            EndTime,
                            DurationSeconds
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
                            EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EndTime")),
                            DurationSeconds = reader.IsDBNull(reader.GetOrdinal("DurationSeconds")) ? (double?)null : reader.GetDouble(reader.GetOrdinal("DurationSeconds"))
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
            }, nameof(SelectAllSessionsForYear));

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



        public List<(DateTime Date, double TotalDurationSeconds)> ReadDurationSecondsLast28Days()
        {
            List<(DateTime Date, double TotalDurationSeconds)> dailyDurations = new List<(DateTime Date, double TotalDurationSeconds)>();
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
                        double totalDurationSeconds = reader.GetDouble(reader.GetOrdinal("TotalDurationSeconds"));
                        dailyDurations.Add((sessionDay, totalDurationSeconds));
                    }
                }
            }, nameof(ReadDurationSecondsLast28Days));

            _appLogger.Info($" ReadDurationSecondsLast28Days StartDate: {DateTime.Now.AddDays(-29):yyyy-MM-dd}, EndDate: {DateTime.Now.AddDays(-1):yyyy-MM-dd}.");
            return dailyDurations;
        }










        public int GetUserIdWithMostRecentLogin()
        {
            int userId = 0;
            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = new SQLiteCommand(@"
                    SELECT 
                            UserId
                    FROM
                            UserCredentials
                    WHERE
                            LastLogin IS NOT NULL
                ORDER BY
                            LastLogin DESC
                    LIMIT
                            1",
                                connection);

                object result = command.ExecuteScalar(); // used for returning only a single result set.
                if (result != null && result != DBNull.Value)
                {
                    userId = Convert.ToInt32(result);
                }
            }, nameof(GetUserIdWithMostRecentLogin));

            return userId;
        }
    }
}
