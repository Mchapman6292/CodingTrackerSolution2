﻿using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IQueryBuilders;
using System.Data.SQLite;
using System.Diagnostics;
using System.Text;

namespace CodingTracker.Data.QueryBuilders
{


    public class QueryBuilder : IQueryBuilder
    {
        private readonly IApplicationLogger _appLogger;

        public QueryBuilder(IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
        }

        // Method to construct SQL command, parameters represent table columns & SQL commands.
        // Parameters for this method represent various SQL commands, e.g orderBy = ORDER BY.
        // Column names are capitalized while query parameters use @ & lower case. Ex UserId = column name, @userId = query parameter.
        public string CreateCommandTextForUserCredentials
            (
            List<string> columnsToSelect,
            int userId = 0,
            string? username = null,
            string? passwordHash = null,
            DateTime? lastLoginDate = null,
            string? orderBy = null,
            bool ascending = true,
            string? groupBy = null,
            int? limit = null)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(CreateCommandTextForUserCredentials)).Start())
            {
                _appLogger.Debug($"Starting {nameof(CreateCommandTextForUserCredentials)}, TraceID: {activity.TraceId}");

                // Checking that orderBy and groupBy parameters are valid column names. 
                var validColumns = new HashSet<string> { "UserId", "Username", "PasswordHash", "LastLogin" };

                if (orderBy != null && !validColumns.Contains(orderBy))
                    throw new ArgumentException("Invalid orderBy column.");

                if (groupBy != null && !validColumns.Contains(groupBy))
                    throw new ArgumentException("Invalid groupBy column.");

                if (columnsToSelect == null || columnsToSelect.Count == 0)
                    throw new ArgumentException("At least one column must be specified.", nameof(columnsToSelect));

                try
                {
                    var conditions = new List<string>();
                    if (userId > 0)
                        conditions.Add("UserId = @userId");
                    if (!string.IsNullOrEmpty(username))
                        conditions.Add("Username = @username");
                    if (!string.IsNullOrEmpty(passwordHash))
                        conditions.Add("PasswordHash = @passwordHash");
                    if (lastLoginDate.HasValue)
                        conditions.Add("LastLogin >= @lastLogin");

                    var columns = string.Join(", ", columnsToSelect);
                    var sql = new StringBuilder($"SELECT {columns} FROM UserCredentials");

                    // WHERE 1=1 allows additional conditions with AND without having to check if its the first condition in the WHERE clause.
                    if (conditions.Count > 0)
                    {
                        sql.Append(" WHERE " + (conditions.Count > 1 ? "1=1 AND " : "") + string.Join(" AND ", conditions));
                    }

                    if (!string.IsNullOrEmpty(groupBy))
                        sql.Append($" GROUP BY {groupBy}");

                    if (!string.IsNullOrEmpty(orderBy))
                        sql.Append($" ORDER BY {orderBy} {(ascending ? "ASC" : "DESC")}");

                    if (limit.HasValue)
                        sql.Append($" LIMIT {limit}");

                    stopwatch.Stop();
                    _appLogger.Debug($" {nameof(CreateCommandTextForUserCredentials)} complete, commandText = {sql} TraceID: {activity.TraceId}, Duration: {stopwatch.ElapsedMilliseconds}ms");

                    return sql.ToString();
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Error building query for UserCredentials: {ex}. Time taken: {stopwatch.ElapsedMilliseconds} ms.");
                    throw;
                }
            }
        }


        // Creates the command parameters that are used for the query.
        public void SetCommandParametersForUserCredentials
        (
            SQLiteCommand command,
            int userId = 0,
            string? username = null,
            string? passwordHash = null,
            DateTime? lastLoginDate = null
        )
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(SetCommandParametersForUserCredentials)).Start())
            {
                _appLogger.Debug($"Starting {nameof(SetCommandParametersForUserCredentials)}, TraceID: {activity.TraceId}");

                try
                {
                    if (userId > 0)
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        _appLogger.Debug($"Binding @UserId with value: {userId}");
                    }
                    if (!string.IsNullOrEmpty(username))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        _appLogger.Debug($"Binding @Username with value: {username}");
                    }
                    if (!string.IsNullOrEmpty(passwordHash))
                    {
                        command.Parameters.AddWithValue("@passwordHash", passwordHash);
                        _appLogger.Debug($"Binding @PasswordHash with value: {passwordHash}");
                    }
                    if (lastLoginDate.HasValue)
                    {
                        command.Parameters.AddWithValue("@lastLogin", lastLoginDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                        _appLogger.Debug($"Binding @LastLogin with value: {lastLoginDate.Value.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }

                    stopwatch.Stop();
                    _appLogger.Debug($"{nameof(SetCommandParametersForUserCredentials)} complete, TraceID: {activity.TraceId}, Duration: {stopwatch.ElapsedMilliseconds}ms");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Error setting command parameters: {ex}. Time taken: {stopwatch.ElapsedMilliseconds} ms, TraceID: {activity.TraceId}");
                    throw;
                }
            }
        }





        public string CreateCommandTextForCodingSessions
        (
            List<string> columnsToSelect,
            int sessionId = 0,
            int userId = 0,
            DateTime? startDate = null,
            DateTime? startTime = null,
            DateTime? endDate = null,
            DateTime? endTime = null,
            double? durationSeconds = 0,
            string? durationHHMM = null,
            string? goalHHMM = null,
            int goalReached = 0,
            string? orderBy = null,
            bool ascending = true,
            string? groupBy = null,
            string? sumColumn = null, 
            int? limit = null
        )
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(CreateCommandTextForCodingSessions)).Start())
            {
                _appLogger.Debug($"Starting {nameof(CreateCommandTextForCodingSessions)}, TraceID: {activity.TraceId}");


                var validColumns = new HashSet<string> { "SessionId", "UserId", "StartDate", "StartTime", "EndDate", "EndTime", "DurationSeconds", "DurationHHMM", "GoalHHMM", "GoalReached." };

                // Checks that valid columns are selected.
                if (orderBy != null && !validColumns.Contains(orderBy))
                    throw new ArgumentException("Invalid orderBy column.");

                if (groupBy != null && !validColumns.Contains(groupBy))
                    throw new ArgumentException("Invalid groupBy column.");

                if (columnsToSelect == null || columnsToSelect.Count == 0)
                    throw new ArgumentException("At least one column must be specified.", nameof(columnsToSelect));


                try
                {
                    // building the conditions for the query.
                    var conditions = new List<string>();

                    if (sessionId > 0)
                        conditions.Add("SessionId = @sessionId");
                    if (userId > 0)
                        conditions.Add("UserId = @userId");
                    if (startDate.HasValue)
                        conditions.Add("StartDate = @startDate");
                    if (startTime.HasValue)
                        conditions.Add("StartTime = @startTime");
                    if (endDate.HasValue)
                        conditions.Add("EndDate = @endDate");
                    if (endTime.HasValue)
                        conditions.Add("EndTime = @endTime");
                    if (durationSeconds > 0)
                        conditions.Add("DurationSeconds = @durationSeconds");
                    if (!string.IsNullOrEmpty(durationHHMM))
                        conditions.Add("DurationHHMM = @durationHHMM");
                    if (!string.IsNullOrEmpty(goalHHMM))
                        conditions.Add("GoalHHMM = @goalHHMM");
                    if (sumColumn != null)
                        conditions.Append($"DATE(StartDate), SUM({sumColumn}) AS Total{sumColumn}");


                    string columns = string.Join(", ", columnsToSelect);
                    if (!string.IsNullOrEmpty(sumColumn))
                    {
                        columns = $"SUM({sumColumn}) AS Total{sumColumn}";
                        if (!string.IsNullOrEmpty(groupBy))
                        {
                            columns += $", {groupBy}";
                        }
                    }


                    // Constructs the full query, begins with SELECT & adds WHERE if there are any conditions.
                    var sql = new StringBuilder($"SELECT {columns} FROM CodingSessions ");

                    if (conditions.Count > 0)
                    {
                        sql.Append(" WHERE " + (conditions.Count > 1 ? "1=1 AND " : "") + string.Join(" AND ", conditions));
                    }


                    if (!string.IsNullOrEmpty(groupBy))
                        sql.Append($" GROUP BY {groupBy}");

                    if (!string.IsNullOrEmpty(orderBy))
                        sql.Append($" ORDER BY {orderBy} {(ascending ? "ASC" : "DESC")}");

                    if (limit.HasValue)
                        sql.Append($" LIMIT {limit}");

                    stopwatch.Stop();
                    _appLogger.Debug($" {nameof(CreateCommandTextForCodingSessions)} complete, commandText = {sql} TraceID: {activity.TraceId}, Duration: {stopwatch.ElapsedMilliseconds}ms");

                    return sql.ToString();
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Error building query for UserCredentials: {ex}. Time taken: {stopwatch.ElapsedMilliseconds} ms.");
                    throw;
                }
            }
        }

        public void SetCommandParametersForCodingSessions
        (
            SQLiteCommand command,
            int sessionId = 0,
            int userId = 0,
            DateTime? startDate = null,
            DateTime? startTime = null,
            DateTime? endDate = null,
            DateTime? endTime = null,
            double? durationSeconds = null,
            string? durationHHMM = null,
            string? goalHHMM = null,
            int goalReached = 0
        )
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(SetCommandParametersForCodingSessions)).Start())
            {
                _appLogger.Debug($"Starting {nameof(SetCommandParametersForCodingSessions)}, TraceID: {activity.TraceId}");

                try
                {
                    if (sessionId > 0)
                    {
                        command.Parameters.AddWithValue("@sessionId", sessionId);
                        _appLogger.Debug($"Binding @sessionId with value: {sessionId}");
                    }
                    if (userId > 0)
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        _appLogger.Debug($"Binding @userId with value: {userId}");
                    }

                    if (startDate.HasValue)
                    {
                        command.Parameters.AddWithValue("@startDate", startDate.Value.ToString("yyyy-MM-dd"));
                        _appLogger.Debug($"Binding @startDate with value: {startDate.Value.ToString("yyyy-MM-dd")}");
                    }
                    if (startTime.HasValue)
                    {
                        command.Parameters.AddWithValue("@startTime", startTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                        _appLogger.Debug($"Binding @startTime with value: {startTime.Value.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }
                    if (endDate.HasValue)
                    {
                        command.Parameters.AddWithValue("@endDate", endDate.Value.ToString("yyyy-MM-dd"));
                        _appLogger.Debug($"Binding @endDate with value: {endDate.Value.ToString("yyyy-MM-dd")}");
                    }
                    if (endTime.HasValue)
                    {
                        command.Parameters.AddWithValue("@endTime", endTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                        _appLogger.Debug($"Binding @endTime with value: {endTime.Value.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }

                    if (durationSeconds.HasValue)
                    {
                        command.Parameters.AddWithValue("@durationSeconds", durationSeconds.Value);
                        _appLogger.Debug($"Binding @durationSeconds with value: {durationSeconds.Value}");
                    }
                    if (!string.IsNullOrEmpty(durationHHMM))
                    {
                        command.Parameters.AddWithValue("@durationHHMM", durationHHMM);
                        _appLogger.Debug($"Binding @durationHHMM with value: {durationHHMM}");
                    }
                    if (!string.IsNullOrEmpty(goalHHMM))
                    {
                        command.Parameters.AddWithValue("@goalHHMM", goalHHMM);
                        _appLogger.Debug($"Binding @goalHHMM with value: {goalHHMM}");
                    }
                    if (goalReached != 0)
                    {
                        command.Parameters.AddWithValue("@GoalReached", goalReached);
                        _appLogger.Debug($"Binding @goalReached with value: {goalReached}");
                    }

                    stopwatch.Stop();
                    _appLogger.Debug($"{nameof(SetCommandParametersForCodingSessions)} complete, TraceID: {activity.TraceId}, Duration: {stopwatch.ElapsedMilliseconds} ms");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Error setting command parameters for CodingSessions: {ex}. Time taken: {stopwatch.ElapsedMilliseconds} ms, TraceID: {activity.TraceId}");
                    throw;
                }
            }
        }


    }
}

