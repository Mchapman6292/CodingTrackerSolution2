// Ignore Spelling: sql

using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.ICodingSessions;
using CodingTracker.Common.IQueryBuilders;
using CodingTracker.Logging.AcitivtyExtensions;
using System.Data.Common;
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


        public bool CheckSQLCommandForCodingSession(string sqlCommand, string traceId, string parentId)
        {
            using (var activity = new Activity(nameof(CheckSQLCommandForCodingSession)).Start()) 
            {
                _appLogger.Info($"Starting {nameof(CheckSQLCommandForCodingSession)} ParentID: {parentId}, TraceID: {traceId}."); 

                var validCommands = new HashSet<string> { "SELECT", "UPDATE", "DELETE" }; // HashSet instead of list to ensure no duplicates.

                if (!validCommands.Contains(sqlCommand)) 
                {
                    _appLogger.Error($" Error during {nameof(CheckSQLCommandForCodingSession)} sql command must be one of SELECT, UPDATE, DELETE. Command used: {sqlCommand}. ParentId{parentId}.");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool CheckValidColumnsForCodingSession(List<string> columnsToSelect, string parentId)
        {
            using (var activity = new Activity(nameof(CheckValidColumnsForCodingSession)).Start())
            {
                _appLogger.Info($"Starting {nameof(CheckValidColumnsForCodingSession)} ParentID: {parentId}");

                var validColumns = new HashSet<string> { "SessionId", "UserId", "StartDate", "StartTime", "EndDate", "EndTime", "DurationSeconds", "DurationHHMM", "GoalHHMM", "GoalReached" };

                List<string> invalidColumns = new List<string>();

                bool allValid = columnsToSelect.All(column => validColumns.Contains(column));

                foreach(var column in columnsToSelect) 
                {
                    if(!validColumns.Contains(column))
                    {
                        invalidColumns.Add(column);
                    }
                }
                if(invalidColumns.Any())
                {
                    string invalidColumnsString = string.Join(", ", invalidColumns);
                    _appLogger.Error($"Invalid columns selected for coding Session table. Invalid columns: {invalidColumnsString}. ParentID: {parentId}.");
                    return false;
                }
                return true;
            }
        }

        public bool CheckColumnsAndCommandsForCreateCommandText(string sqlCommand, List<string> columnsToSelect, string traceId, string parentId, string spanId, string orderBy, string groupBy)
        {
            using (var activity = new Activity(nameof(CheckColumnsAndCommandsForCreateCommandText)).Start())
            {
                _appLogger.Debug($"Starting {nameof(CheckColumnsAndCommandsForCreateCommandText)} TraceID: {activity.TraceId}.");

                var validColumns = new HashSet<string> { "SessionId", "UserId", "StartDate", "StartTime", "EndDate", "EndTime", "DurationSeconds", "DurationHHMM", "GoalHHMM", "GoalReached" };

                bool isvalidCommands = CheckSQLCommandForCodingSession(sqlCommand, activity.TraceId.ToString(), activity.TraceId.ToString());
                bool isValidColumns = CheckValidColumnsForCodingSession(columnsToSelect, activity.ParentId.ToString());


                if (orderBy != null && !validColumns.Contains(orderBy))
                {
                    _appLogger.Error($"Invalid orderBy columns ParentID: {activity.ParentId}.");
                    return false;
                }

                if (groupBy != null && !validColumns.Contains(groupBy))
                {
                    _appLogger.Error($"Invalid groupBy columns ParentID: {activity.ParentId}.");
                    return false;
                }


                if (columnsToSelect == null || columnsToSelect.Count == 0)
                {
                    _appLogger.Error("At least one column must be specified.", nameof(columnsToSelect));
                    return false;
                }
                return true;
            }
        }

        public string CreateCommandTextForCodingSessions
        (
         List<string> columnsToSelect,
            string sqlCommand,
            int sessionId = 0,
            int userId = 0,
            DateOnly startDate,
            DateTime startTime,
            DateOnly endDate,
            DateTime endTime,
            double durationSeconds,
            string durationHHMM,
            string goalHHMM,
            int goalReached = 0,
            string orderBy = "",
            bool ascending = true,
            string groupBy = "",
            string sumColumn = "",
            int limit = 10
        )
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(CreateCommandTextForCodingSessions)).Start())
            {
                _appLogger.Debug($"Starting {nameof(CreateCommandTextForCodingSessions)}, TraceID: {activity.TraceId}");

                if (!CheckColumnsAndCommandsForCreateCommandText(sqlCommand, columnsToSelect, activity.TraceId.ToString(), activity.ParentId, activity.SpanId.ToString(), orderBy, groupBy))
                {
                    throw new ArgumentException($"Invalid columns/SQL commands selected for {nameof(CreateCommandTextForCodingSessions)}");
                }
                var sql = new StringBuilder();

                // switch for sql command
                switch (sqlCommand)
                {
                    case "SELECT":
                        string commandColumns = string.Join(", ", columnsToSelect);
                        sql.Append($"SELECT {commandColumns} FROM CodingSessions");
                        break;
                    case "UPDATE":
                        sql.Append("UPDATE CodingSessions SET ");
                        sql.Append(string.Join(", ", columnsToSelect.Select(col => $"{col} = @{col}")));
                        if (sessionId > 0 || userId > 0) 
                            sql.Append(" WHERE ");
                        break;
                    case "DELETE":
                        sql.Append("DELETE FROM CodingSessions");
                        break;
                }

                _appLogger.Info($"Current sql: {sql}");

                try
                {
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


        public string CreateInsertTextForCodingSessions(int userId, DateOnly startDate, DateTime startTime, DateOnly endDate, DateTime endTime, double durationSeconds, string durationHHMM, string goalHHMM, int goalReached)
        {
            ActivityTraceId traceId = default;
            string sqlCommand = "";
            Stopwatch stopwatch = Stopwatch.StartNew();
            _appLogger.LogActivity(nameof(CreateInsertTextForCodingSessions), activity =>
            {
                ActivityTraceId traceId = activity.TraceId;
                _appLogger.Info($"Starting {nameof(CreateInsertTextForCodingSessions)}, TraceID:{activity.TraceId}.");
                _appLogger.Info($"Parameters for {nameof(CreateInsertTextForCodingSessions)}: UserId = {userId}, StartDate = {startDate}, StartTime = {startTime}, EndDate = {endDate}, EndTime = {endTime}, DurationSeconds = {durationSeconds}, DurationHHMM = {durationHHMM}, GoalHHMM = {goalHHMM}, GoalReached = {goalReached}");

            }, activity =>
            {
                try
                {
                    sqlCommand = $"INSERT INTO CodingSessions (UserId, StartDate, StartTime, EndDate, EndTime, DurationSeconds, DurationHHMM, GoalHHMM, GoalReached) VALUES (@UserId, @StartDate, @StartTime, @EndDate, @EndTime, @DurationSeconds, @DurationHHMM, @GoalHHMM, @GoalReached);";
                    _appLogger.Info($"CommandText for {nameof(CreateInsertTextForCodingSessions)}: {sqlCommand}. TraceID:{activity.TraceId}.");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Error in {nameof(CreateInsertTextForCodingSessions)}: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {traceId}");
                    throw;
                }
            }, activity =>
            {
                stopwatch.Stop();
                _appLogger.Info($"Completed {nameof(CreateInsertTextForCodingSessions)} Execution Time: {stopwatch.ElapsedMilliseconds}. TraceID: {activity.TraceId}.");
            });
            return sqlCommand;
        }

        


        public bool CheckValuesForInsertParametersAreValid
        (
            int userId,
            DateOnly startDate,
            DateTime startTime,
            DateOnly endDate,
            DateTime endTime,
            double durationSeconds,
            string durationHHMM,
            string goalHHMM,
            int goalReached
        )
        {
            return _appLogger.LogActivity(nameof(CheckValuesForInsertParametersAreValid), activity =>
            {
                List<string> errorMessages = new List<string>();

                if (userId == 0)
                    errorMessages.Add($"UserId is set to 0, which is the default for not set.");

                if (startDate == DateOnly.MinValue)
                    errorMessages.Add($"StartDate is set to MinValue, which is the default for not set.");

                if (startTime == DateTime.MinValue)
                    errorMessages.Add($"StartTime is set to MinValue, which is the default for not set.");

                if (endDate == DateOnly.MaxValue)
                    errorMessages.Add($"EndDate is set to MaxValue, which is the default for not set.");

                if (endTime == DateTime.MaxValue)
                    errorMessages.Add($"EndTime is set to MaxValue, which is the default for not set.");

                if (durationSeconds < 0)
                    errorMessages.Add($"DurationSeconds is negative, which indicates it is not properly set.");

                if (string.IsNullOrEmpty(durationHHMM))
                    errorMessages.Add($"DurationHHMM is empty or null, which is not expected.");

                if (string.IsNullOrEmpty(goalHHMM))
                    errorMessages.Add($"GoalHHMM is empty or null, which is not expected.");

                if (goalReached == 0)
                    errorMessages.Add($"GoalReached is set to 0, which is the default for not set.");

                if (errorMessages.Count > 0)
                {
                    string fullErrorMessage = string.Join(Environment.NewLine, errorMessages);
                    _appLogger.Error($"Validation errors: {fullErrorMessage}. TraceID: {activity.TraceId}");
                    return false;
                }
                return true; 
            });
        }


        public void SetCommandParametersForInsertCodingSessions
       (
           SQLiteCommand command,
           int userId,
           DateOnly startDate,
           DateTime startTime,
           DateOnly endDate,
           DateTime endTime,
           double durationSeconds,
           string durationHHMM,
           string goalHHMM,
           int goalReached
       )
        {
            _appLogger.LogActivity(nameof(SetCommandParametersForInsertCodingSessions), activity =>
            {
                ActivityTraceId traceId = activity.TraceId;
                _appLogger.Info($"Starting {nameof(SetCommandParametersForInsertCodingSessions)}. TraceID:{activity.TraceId}.");
            },
            activity =>
            {
                try
                {
                    if (CheckValuesForInsertParametersAreValid(userId, startDate, startTime, endDate, endTime, durationSeconds, durationHHMM, goalHHMM, goalReached) == false)
                    {
                        throw new InvalidOperationException("Invalid values for CheckValuesForInsertParametersAreValid");
                    }
                    else
                    {
                        if (userId != 0)
                        {
                            command.Parameters.AddWithValue("@userId", userId);
                            _appLogger.Info($"Binding @userId with value: {userId}");
                        }
                        if (startDate != DateOnly.MinValue)
                        {
                            command.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                            _appLogger.Info($"Binding @startDate with value: {startDate.ToString("yyyy-MM-dd")}");
                        }
                        if (startTime != DateTime.MinValue)
                        {
                            command.Parameters.AddWithValue("@startTime", startTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            _appLogger.Info($"Binding @startTime with value: {startTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                        }
                        if (endDate != DateOnly.MaxValue)
                        {
                            command.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));
                            _appLogger.Info($"Binding @endDate with value: {endDate.ToString("yyyy-MM-dd")}");
                        }
                        if (endTime != DateTime.MinValue)
                        {
                            command.Parameters.AddWithValue("@endTime", endTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            _appLogger.Info($"Binding @endTime with value: {endTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                        }
                        if (durationSeconds >= 0) 
                        {
                            command.Parameters.AddWithValue("@durationSeconds", durationSeconds);
                            _appLogger.Info($"Binding @durationSeconds with value: {durationSeconds}");
                        }
                        if (!string.IsNullOrEmpty(durationHHMM) && durationHHMM != "00:00")
                        {
                            command.Parameters.AddWithValue("@durationHHMM", durationHHMM);
                            _appLogger.Info($"Binding @durationHHMM with value: {durationHHMM}");
                        }
                        if (!string.IsNullOrEmpty(goalHHMM) && goalHHMM != "00:00")
                        {
                            command.Parameters.AddWithValue("@goalHHMM", goalHHMM);
                            _appLogger.Info($"Binding @goalHHMM with value: {goalHHMM}");
                        }
                        if (goalReached != 0)
                        {
                            command.Parameters.AddWithValue("@goalReached", goalReached);
                            _appLogger.Info($"Binding @goalReached with value: {goalReached}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"Exception in {nameof(SetCommandParametersForInsertCodingSessions)}: {ex.Message}. TraceID: {activity.TraceId}");
                    throw;
                }
            });
        }



        public void SetCommandParametersForCodingSessions
        (
            SQLiteCommand command,
            int sessionId,
            int userId,
            DateOnly startDate,
            DateTime startTime,
            DateOnly endDate,
            DateTime endTime,
            double durationSeconds,
            string durationHHMM,
            string goalHHMM,
            int goalReached
        )
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(SetCommandParametersForCodingSessions)).Start())
            {
                _appLogger.Debug($"Starting {nameof(SetCommandParametersForCodingSessions)}, TraceID: {activity.TraceId}");

                try
                {
                    command.Parameters.AddWithValue("@sessionId", sessionId);
                    _appLogger.Debug($"Binding @sessionId with value: {sessionId}");

                    command.Parameters.AddWithValue("@userId", userId);
                    _appLogger.Debug($"Binding @userId with value: {userId}");

                    command.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                    _appLogger.Debug($"Binding @startDate with value: {startDate.ToString("yyyy-MM-dd")}");

                    command.Parameters.AddWithValue("@startTime", startTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    _appLogger.Debug($"Binding @startTime with value: {startTime.ToString("yyyy-MM-dd HH:mm:ss")}");

                    command.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));
                    _appLogger.Debug($"Binding @endDate with value: {endDate.ToString("yyyy-MM-dd")}");

                    command.Parameters.AddWithValue("@endTime", endTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    _appLogger.Debug($"Binding @endTime with value: {endTime.ToString("yyyy-MM-dd HH:mm:ss")}");

                    command.Parameters.AddWithValue("@durationSeconds", durationSeconds);
                    _appLogger.Debug($"Binding @durationSeconds with value: {durationSeconds}");

                    command.Parameters.AddWithValue("@durationHHMM", durationHHMM);
                    _appLogger.Debug($"Binding @durationHHMM with value: {durationHHMM}");

                    command.Parameters.AddWithValue("@goalHHMM", goalHHMM);
                    _appLogger.Debug($"Binding @goalHHMM with value: {goalHHMM}");

                    command.Parameters.AddWithValue("@goalReached", goalReached);
                    _appLogger.Debug($"Binding @goalReached with value: {goalReached}");

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

