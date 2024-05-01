using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Data.QueryBuilders;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IDatabaseManagers;
using System.Data.SQLite;
using CodingTracker.Common.IQueryBuilders;
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.INewDatabaseReads;
using System.Diagnostics;

namespace CodingTracker.Data.NewDatabaseReads
{
    public class NewDatabaseRead : INewDatabaseRead
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseManager _databaseManager;
        private readonly IQueryBuilder _queryBuilder;


        public NewDatabaseRead(IApplicationLogger appLogger, IDatabaseManager databaseManager, IQueryBuilder queryBuilder)
        {
            _appLogger = appLogger;
            _databaseManager = databaseManager;
            _queryBuilder = queryBuilder;
        }


        public List<UserCredentialDTO> ReadFromUserCredentialsTable
(
     List<string> columnsToSelect,
     int userId = 0,
     string? username = null,
     string? passwordHash = null,
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
                        $"LastLoginDate={lastLoginDate}, OrderBy={orderBy}, Ascending={ascending}, GroupBy={groupBy}, Limit={limit}");

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
                                    if (reader.HasRows)
                                    {
                                        userCredentials.Add(ExtractUserCredentialFromReader(reader));
                                    }
                                    else
                                    {
                                        _appLogger.Debug("No rows found to read.");
                                    }
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

                    if (!reader.IsDBNull(userIdIndex))
                        dto.UserId = reader.GetInt32(userIdIndex); 
                    else
                        dto.UserId = 0; 

 
                    dto.Username = reader.IsDBNull(usernameIndex) ? "" : reader.GetString(usernameIndex); 

   
                    dto.PasswordHash = reader.IsDBNull(passwordHashIndex) ? "" : reader.GetString(passwordHashIndex); 

                    if (!reader.IsDBNull(lastLoginIndex))
                        dto.LastLogin = reader.GetDateTime(lastLoginIndex); // Nullable in DTO, handle appropriately

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
                   bool aggregateDurationsByDate = false,
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
                        string commandText = _queryBuilder.CreateCommandTextForCodingSessions(columnsToSelect, sessionId, userId, startDate, startTime, endDate, endTime, orderBy, ascending, groupBy, sumColumn, limit);
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
                    throw; // Rethrow the exception after logging it
                }
            }
        }


    }
}

