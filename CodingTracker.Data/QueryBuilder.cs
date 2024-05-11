// Ignore Spelling: sql

using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.ICodingSessions;
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

        // Method to construct SQL command, parameters represent table commandColumns & SQL commands.
        // Parameters for this method represent various SQL commands, e.g orderBy = ORDER BY.
        // Column names are capitalized while query parameters use @ & lower case. Ex userId = column name, @userId = query parameter.
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

                var validCommands = new HashSet<string> { "SELECT", "INSERT", "UPDATE", "DELETE" };

  

                // Checking that orderBy and groupBy parameters are valid column names. 
                var validColumns = new HashSet<string> { "userId", "Username", "PasswordHash", "LastLogin" };

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
                        conditions.Add("userId = @userId");
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
                        _appLogger.Debug($"Binding @userId with value: {userId}");
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
            string sqlCommand,
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


                // sql command checks
                var validCommands = new HashSet<string> { "SELECT", "INSERT", "UPDATE", "DELETE" };

                if (!validCommands.Contains(sqlCommand.ToUpper()))
                {
                    _appLogger.Error($"Invalid SQL command for {nameof(CreateCommandTextForUserCredentials)}. sqlCommand = {sqlCommand}.");
                    throw new ArgumentException("Invalid SQL command specified.", nameof(sqlCommand));
                }
                // valid commandColumns checks
                var validColumns = new HashSet<string> { "SessionId", "UserId", "StartDate", "StartTime", "EndDate", "EndTime", "DurationSeconds", "DurationHHMM", "GoalHHMM", "GoalReached" };

                if (!columnsToSelect.All(col => validColumns.Contains(col)))
                    throw new ArgumentException($"Invalid column(s) specified.", nameof(columnsToSelect));

       
                if (orderBy != null && !validColumns.Contains(orderBy))
                    throw new ArgumentException("Invalid orderBy column.");

                if (groupBy != null && !validColumns.Contains(groupBy))
                    throw new ArgumentException("Invalid groupBy column.");

                if (columnsToSelect == null || columnsToSelect.Count == 0)
                    throw new ArgumentException("At least one column must be specified.", nameof(columnsToSelect));

                var sql = new StringBuilder();

                // switch for sql command
                switch (sqlCommand)
                {
                    case "SELECT":
                        string commandColumns = string.Join(", ", columnsToSelect);
                        sql.Append($"SELECT {commandColumns} FROM CodingSessions");
                        break;
                    case "INSERT":
                        sql.Append($"INSERT INTO CodingSessions ({string.Join(", ", columnsToSelect)}) VALUES ({string.Join(", ", columnsToSelect.Select(col => $"@{col}"))})");
                        break;
                    case "UPDATE":
                        sql.Append("UPDATE CodingSessions SET ");
                        sql.Append(string.Join(", ", columnsToSelect.Select(col => $"{col} = @{col}")));
                        if (sessionId > 0 || userId > 0) // Using conditions to restrict the update, this part will be detailed in the next section.
                            sql.Append(" WHERE ");
                        break;
                    case "DELETE":
                        sql.Append("DELETE FROM CodingSessions");
                        break;
                }

                _appLogger.Info($"Current sql: {sql}");

                try
                {
                    // building the conditions for the query.
                    var conditions = new List<string>();

                    if (sessionId > 0)
                        conditions.Add("sessionId = @sessionId");
                    if (userId > 0)
                        conditions.Add("userId = @userId");
                    if (startDate.HasValue)
                        conditions.Add("startDate = @startDate");
                    if (startTime.HasValue)
                        conditions.Add("startTime = @startTime");
                    if (endDate.HasValue)
                        conditions.Add("endDate = @endDate");
                    if (endTime.HasValue)
                        conditions.Add("endTime = @endTime");
                    if (durationSeconds > 0)
                        conditions.Add("durationSeconds = @durationSeconds");
                    if (!string.IsNullOrEmpty(durationHHMM))
                        conditions.Add("durationHHMM = @durationHHMM");
                    if (!string.IsNullOrEmpty(goalHHMM))
                        conditions.Add("goalHHMM = @goalHHMM");


                    string columns = string.Join(", ", columnsToSelect);
                    if (!string.IsNullOrEmpty(sumColumn))
                    {
                        columns = $"SUM({sumColumn}) AS Total{sumColumn}";  
                        if (!string.IsNullOrEmpty(groupBy))
                        {
                            columns += $", {groupBy}";
                        }
                    }
                    if (conditions.Count > 0)
                    {
                        if (sqlCommand != "INSERT" && sqlCommand != "DELETE")
                        {
                            sql.Append(" WHERE " + (conditions.Count > 1 ? "1=1 AND " : "") + string.Join(" AND ", conditions));
                        }
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
                        command.Parameters.AddWithValue("@goalReached", goalReached);
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

