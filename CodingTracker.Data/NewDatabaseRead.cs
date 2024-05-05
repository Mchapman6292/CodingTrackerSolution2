using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Data.QueryBuilders;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IDatabaseManagers;
using System.Data.SQLite;
using CodingTracker.Common.IQueryBuilders;
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.INewDatabaseReads;
using CodingTracker.Common.CodingGoalDTOManagers;
using CodingTracker.Common.CodingSessionDTOManagers;
using CodingTracker.Common.UserCredentialDTOManagers;
using System.Diagnostics;

namespace CodingTracker.Data.NewDatabaseReads
{
    public class NewDatabaseRead : INewDatabaseRead
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseManager _databaseManager;
        private readonly IQueryBuilder _queryBuilder;
        private readonly ICodingGoalDTOManager _codingGoalDTOManager;
        private readonly ICodingSessionDTOManager _codingSessionDTOManager;
        private readonly IUserCredentialDTOManager _userCredentialDTOManager;


        public NewDatabaseRead(IApplicationLogger appLogger, IDatabaseManager databaseManager, IQueryBuilder queryBuilder, ICodingGoalDTOManager codingGoalDTOManager, ICodingSessionDTOManager codingSessionDTOManager)
        {
            _appLogger = appLogger;
            _databaseManager = databaseManager;
            _queryBuilder = queryBuilder;
            _codingGoalDTOManager = codingGoalDTOManager;
            _codingSessionDTOManager = codingSessionDTOManager;
        }


        public List<UserCredentialDTO> ReadFromUserCredentialsTable
        (
             List<string> columnsToSelect,
             int userId = 0,
             string username = null,
             string passwordHash = null,
             DateTime? lastLoginDate = null,
             string? orderBy = null,
             bool ascending = true,
             string? groupBy = null,
             int? limit = null
        )
        {
            List<UserCredentialDTO> userCredentials = new List<UserCredentialDTO>();
            using (var activity = new Activity(nameof(ReadFromUserCredentialsTable)).Start())
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Info($"Starting {nameof(ReadFromUserCredentialsTable)}. TraceID: {activity.TraceId}");

                _appLogger.Debug($"Parameters: UserId={userId}, Username={username}, PasswordHash={passwordHash}, " +
                        $"LastLoginDate={lastLoginDate}, OrderBy={orderBy}, Ascending={ascending}, GroupBy={groupBy}, Limit={limit}. TraceID: {activity.TraceId}");

                try
                {
                    _databaseManager.ExecuteDatabaseOperation(connection =>
                    {
                        string commandText = _queryBuilder.CreateCommandTextForUserCredentials(
                            columnsToSelect, userId, username, passwordHash, lastLoginDate, orderBy, ascending, groupBy, limit);

                        using (var command = new SQLiteCommand(commandText, connection))
                        {
                            _queryBuilder.SetCommandParametersForUserCredentials(command, userId, username, passwordHash, lastLoginDate);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    foreach (var column in columnsToSelect)
                                    {
                                        int columnIndex = reader.GetOrdinal(column);
                                        string columnValue = reader.IsDBNull(columnIndex) ? "NULL" : reader.GetValue(columnIndex).ToString();
                                        _appLogger.Debug($"Column {column}: {columnValue}");
                                    }

                                    userCredentials.Add(ExtractUserCredentialFromReader(reader));
                                }
                            }
                        }

                        stopwatch.Stop();
                        _appLogger.Info($"Completed {nameof(ReadFromUserCredentialsTable)}. Retrieved {userCredentials.Count} user credentials. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }, nameof(ReadFromUserCredentialsTable));
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Error in {nameof(ReadFromUserCredentialsTable)}: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    throw;
                }
            }

            foreach (var cred in userCredentials)
            {
                _appLogger.Info($"User Credential: UserId={cred.UserId}, Username={cred.Username}, PasswordHash={cred.PasswordHash}, LastLogin={cred.LastLogin}");
            }
            return userCredentials;
        }

        private UserCredentialDTO ExtractUserCredentialFromReader(SQLiteDataReader reader)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            using (var activity = new Activity(nameof(ExtractUserCredentialFromReader)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ExtractUserCredentialFromReader)}. TraceID: {activity.TraceId}");

                try
                {
                    var dto = new UserCredentialDTO();

                    int userIdIndex = reader.GetOrdinal("UserId");
                    int usernameIndex = reader.GetOrdinal("Username");
                    int passwordHashIndex = reader.GetOrdinal("PasswordHash");
                    int lastLoginIndex = reader.GetOrdinal("LastLogin");

                    string dtoUsername = reader.IsDBNull(usernameIndex) ? "" : reader.GetString(usernameIndex);
                    string dtoPasswordHash = reader.IsDBNull(passwordHashIndex) ? "" : reader.GetString(passwordHashIndex);
                    DateTime LastLogin = reader.IsDBNull(lastLoginIndex) ? DateTime.UtcNow : reader.GetDateTime(lastLoginIndex);



                    if (!reader.IsDBNull(userIdIndex))
                        dto.UserId = reader.GetInt32(userIdIndex);
                    else
                        _appLogger.Debug($"UserId from reader is null. TraceID: {activity.TraceId}");

                    if (!reader.IsDBNull(usernameIndex))
                        dto.Username = reader.GetString(usernameIndex);
                    else
                        _appLogger.Debug($"Username from reader is null. TraceID: {activity.TraceId}");
                    if (!reader.IsDBNull(passwordHashIndex))
                        dto.PasswordHash = reader.GetString(passwordHashIndex);

                    if (!reader.IsDBNull(lastLoginIndex))
                        dto.LastLogin = LastLogin;
                    else
                        _appLogger.Debug($"LastLogin from reader is null. TraceID: {activity.TraceId}");

                    _appLogger.Debug($"dtoUsername set {dtoUsername}, passwordHash {dtoPasswordHash}.");

                    stopwatch.Stop();
                    _appLogger.Info($"{nameof(ExtractUserCredentialFromReader)} completed successfully. TraceID: {activity.TraceId}, Duration: {stopwatch.ElapsedMilliseconds}ms");

                    return dto;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Error in {nameof(ExtractUserCredentialFromReader)}: {ex.Message}. TraceID: {activity.TraceId}, Duration: {stopwatch.ElapsedMilliseconds}ms");
                    throw;
                }
            }
        }







            public List<CodingSessionDTO> ReadFromCodingSessionsTable
               (
                   List<string> columnsToSelect,
                   int sessionId = 0,
                   int userId = 0,
                   DateTime? startDate = null,
                   DateTime? startTime = null,
                   DateTime? endDate = null,
                   DateTime? endTime = null,
                   double? durationSeconds = null,
                   string? durationHHMM = null,
                   string? goalHHMM = null,
                   int goalReached = 0,
                   string? orderBy = null,
                   bool ascending = true,
                   string? groupBy = null,
                   string? sumColumn = null, // New parameter to specify which column to sum
                   int? limit = null
               )
        {
            List<CodingSessionDTO> codingSessions = new List<CodingSessionDTO>();
            using (var activity = new Activity(nameof(ReadFromCodingSessionsTable)).Start())
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Info($"Starting {nameof(ReadFromCodingSessionsTable)}. TraceID: {activity.TraceId}");

                _appLogger.Debug($"Parameters: SessionId={sessionId}, UserId={userId}, StartDate={startDate}, StartTime={startTime}, " +
                        $"EndDate={endDate}, EndTime={endTime}, OrderBy={orderBy}, Ascending={ascending}, GroupBy={groupBy}, " +
                        $"SumColumn={sumColumn}, Limit={limit}");

                try
                {
                    _databaseManager.ExecuteDatabaseOperation(connection =>
                    {
                        string commandText = _queryBuilder.CreateCommandTextForCodingSessions(columnsToSelect, sessionId, userId, startDate, startTime, endDate, endTime, durationSeconds, durationHHMM, goalHHMM, goalReached, orderBy, ascending, groupBy, sumColumn, limit);
                        using (var command = new SQLiteCommand(commandText, connection)) // Uses the text created by CreatecommandText to generate a new command object.
                        {
                            _queryBuilder.SetCommandParametersForCodingSessions(command);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    codingSessions.Add(ExtractCodingSessionFromReader(reader));
                                }
                            }
                        }
                    }, nameof(ReadFromCodingSessionsTable));

                    stopwatch.Stop();
                    _appLogger.Info($"Completed {nameof(ReadFromCodingSessionsTable)}. Retrieved {codingSessions.Count} coding sessions. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Error in {nameof(ReadFromCodingSessionsTable)}: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    throw;
                }
            }

            return codingSessions;
        }




        private CodingSessionDTO ExtractCodingSessionFromReader(SQLiteDataReader reader)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(ExtractCodingSessionFromReader)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ExtractCodingSessionFromReader)}. TraceID: {activity.TraceId}");

                try
                {
                    var dto = new CodingSessionDTO();

                    int sessionIdIndex = reader.GetOrdinal("SessionId");
                    if (!reader.IsDBNull(sessionIdIndex))
                        dto.SessionId = reader.GetInt32(sessionIdIndex);

                    int userIdIndex = reader.GetOrdinal("UserId");
                    if (!reader.IsDBNull(userIdIndex))
                        dto.UserId = reader.GetInt32(userIdIndex);

                    int startDateIndex = reader.GetOrdinal("StartDate");
                    if (!reader.IsDBNull(startDateIndex))
                        dto.StartDate = reader.GetDateTime(startDateIndex);

                    int startTimeIndex = reader.GetOrdinal("StartTime");
                    if (!reader.IsDBNull(startTimeIndex))
                        dto.StartTime = reader.GetDateTime(startTimeIndex);

                    int endDateIndex = reader.GetOrdinal("EndDate");
                    if (!reader.IsDBNull(endDateIndex))
                        dto.EndDate = reader.GetDateTime(endDateIndex);

                    int endTimeIndex = reader.GetOrdinal("EndTime");
                    if (!reader.IsDBNull(endTimeIndex))
                        dto.EndTime = reader.GetDateTime(endTimeIndex);

                    int durationSecondsIndex = reader.GetOrdinal("DurationSeconds");
                    if (!reader.IsDBNull(durationSecondsIndex))
                        dto.DurationSeconds = reader.GetDouble(durationSecondsIndex);

                    int durationHHMMIndex = reader.GetOrdinal("DurationHHMM");
                    if (!reader.IsDBNull(durationHHMMIndex))
                        dto.DurationHHMM = reader.GetString(durationHHMMIndex);

                    int goalHHMMIndex = reader.GetOrdinal("GoalHHMM");
                    if (!reader.IsDBNull(goalHHMMIndex))
                        dto.GoalHHMM = reader.GetString(goalHHMMIndex);

                    int goalReachedIndex = reader.GetOrdinal("GoalReached");
                    if (!reader.IsDBNull(goalReachedIndex))
                        dto.GoalReached = reader.GetInt32(goalReachedIndex);

                    stopwatch.Stop();
                    _appLogger.Info($"{nameof(ExtractCodingSessionFromReader)} completed successfully. TraceID: {activity.TraceId}, Duration: {stopwatch.ElapsedMilliseconds}ms");

                    return dto;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Error in {nameof(ExtractCodingSessionFromReader)}: {ex.Message}. TraceID: {activity.TraceId}, Duration: {stopwatch.ElapsedMilliseconds}ms");
                    throw;
                }
            }
        }


    }
}

