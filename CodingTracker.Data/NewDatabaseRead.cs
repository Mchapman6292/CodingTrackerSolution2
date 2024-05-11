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
using System.ComponentModel.DataAnnotations.Schema;

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
        private UserCredentialDTO _currentUserCredential;


        public NewDatabaseRead(IApplicationLogger appLogger, IDatabaseManager databaseManager, IQueryBuilder queryBuilder, ICodingGoalDTOManager codingGoalDTOManager, ICodingSessionDTOManager codingSessionDTOManager)
        {
            _appLogger = appLogger;
            _databaseManager = databaseManager;
            _queryBuilder = queryBuilder;
            _codingGoalDTOManager = codingGoalDTOManager;
            _codingSessionDTOManager = codingSessionDTOManager;
        }


        public List<UserCredentialDTO> HandleUserCredentialsOperations
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
            _appLogger.LogActivity(nameof(HandleUserCredentialsOperations), activity =>
            {
                ActivityTraceId traceId = activity.TraceId;
                _appLogger.Info($"Starting {nameof(HandleUserCredentialsOperations)}. TraceID: {traceId}");
                _appLogger.Debug($"Parameters: userId={userId}, Username={username}, PasswordHash=HIDDEN, " +
                        $"LastLoginDate={lastLoginDate}, OrderBy={orderBy}, Ascending={ascending}, GroupBy={groupBy}, Limit={limit}. TraceID: {traceId}");

            },
            activity => // The second lambda of LogActivity() requires no parameters unlike the starting and ending lambdas
            {
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    // Database operation
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
                                    var credential = ExtractUserCredentialFromReader(reader);
                                    userCredentials.Add(credential);
                                    _appLogger.Debug($"Read UserCredential: userId={credential.UserId}, Username={credential.Username}, PasswordHash=HIDDEN");
                                }
                            }
                        }
                    }, nameof(HandleUserCredentialsOperations));
                    stopwatch.Stop();
                    _appLogger.Info($"Completed {nameof(HandleUserCredentialsOperations)}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {Activity.Current?.TraceId}");
                }
                catch (Exception ex)
                {
                    _appLogger.LogDatabaseError(ex, nameof(HandleUserCredentialsOperations), stopwatch);
                    throw;
                }
            });
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
                    var dto = new UserCredentialDTO
                    {
                        UserId = reader.IsDBNull(reader.GetOrdinal("userId")) ? 0 : reader.GetInt32(reader.GetOrdinal("userId")),
                        Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? null : reader.GetString(reader.GetOrdinal("Username")),
                        PasswordHash = reader.IsDBNull(reader.GetOrdinal("PasswordHash")) ? null : reader.GetString(reader.GetOrdinal("PasswordHash")),
                        LastLogin = reader.IsDBNull(reader.GetOrdinal("LastLogin")) ? DateTime.UtcNow : reader.GetDateTime(reader.GetOrdinal("LastLogin"))
                    };

                    _appLogger.Debug($"UserCredentialDTO created - userId: {dto.UserId}, Username: {dto.Username}, PasswordHash: {dto.PasswordHash}, LastLogin: {dto.LastLogin}. TraceID: {activity.TraceId}");

                    stopwatch.Stop();
                    _appLogger.Info($"{nameof(ExtractUserCredentialFromReader)} completed successfully.  Duration: {stopwatch.ElapsedMilliseconds}ms, TraceID: {activity.TraceId}");

                    return dto;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Error in {nameof(ExtractUserCredentialFromReader)}: {ex.Message}. Duration: {stopwatch.ElapsedMilliseconds}ms, TraceID: {activity.TraceId}.");
                    throw;
                }
            }
        }







        public List<CodingSessionDTO> HandleCodingSessionsTableOperations
        (

            List<string> columnsToSelect,
            string sqlCommand,
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
            ActivityTraceId traceId = default;
            _appLogger.LogActivity(nameof(HandleCodingSessionsTableOperations), activity =>
            {
                traceId = activity.TraceId;
                _appLogger.Info($"Starting {nameof(HandleCodingSessionsTableOperations)}. TraceID: {activity.TraceId}");
                _appLogger.Debug($"Parameters: sessionId={sessionId}, userId={userId}, startDate={startDate}, startTime={startTime}, " +
                $"endDate={endDate}, endTime={endTime}, durationSeconds={durationSeconds}, durationHHMM={durationHHMM}, " +
                $"goalHHMM={goalHHMM}, goalReached={goalReached}, OrderBy={orderBy}, Ascending={ascending}, GroupBy={groupBy}, " +
                $"SumColumn={sumColumn}, Limit={limit}");

            }, activity =>
            {
                
                var stopwatch = Stopwatch.StartNew();
                try
                    {
                        _databaseManager.ExecuteDatabaseOperation(connection =>
                        {
  
                            string commandText = _queryBuilder.CreateCommandTextForCodingSessions(columnsToSelect, sqlCommand, sessionId, userId, startDate, startTime, endDate, endTime, durationSeconds, durationHHMM, goalHHMM, goalReached);
                            using (var command = new SQLiteCommand(commandText, connection)) // Uses the text created by CreatecommandText to generate a new command object.
                            {
                                _queryBuilder.SetCommandParametersForCodingSessions(command, sessionId, userId, startDate, startTime, endDate, endTime, durationSeconds, durationHHMM, goalHHMM, goalReached);

                                if (sqlCommand == "SELECT")
                                {
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
                                            codingSessions.Add(ExtractCodingSessionFromReader(reader));
                                        }
                                    }
                                }
                                else
                                {
                                    command.Parameters.AddWithValue(@"SessionId", sessionId);
                                    command.Parameters.AddWithValue("@StartDate", startDate.HasValue ? (object)startDate.Value : DBNull.Value);
                                    command.Parameters.AddWithValue("@StartTime", startTime.HasValue ? (object)startTime.Value : DBNull.Value);
                                    command.Parameters.AddWithValue("@EndDate", endDate.HasValue ? (object)endDate.Value : DBNull.Value);
                                    command.Parameters.AddWithValue("@EndTime", endTime.HasValue ? (object)endTime.Value : DBNull.Value);
                                    command.Parameters.AddWithValue("@DurationSeconds", durationSeconds.HasValue ? (object)durationSeconds.Value : DBNull.Value);
                                    command.Parameters.AddWithValue("@DurationHHMM", durationHHMM ?? (object)DBNull.Value);
                                    command.Parameters.AddWithValue("@GoalHHMM", goalHHMM ?? (object)DBNull.Value);
                                    command.Parameters.AddWithValue("@GoalReached", goalReached);

                                    int affectedRows = command.ExecuteNonQuery();
                                    _appLogger.Debug($"Affected rows: {affectedRows}, TraceId: {traceId} ");
                                }
                            }
                        }, nameof(HandleCodingSessionsTableOperations));
                
                        stopwatch.Stop();
                        _appLogger.Info($"Completed {nameof(HandleCodingSessionsTableOperations)}. Retrieved {codingSessions.Count} coding sessions. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {traceId}");
                    }
                    catch (Exception ex)
                    {
                        stopwatch.Stop();
                        _appLogger.Error($"Error in {nameof(HandleCodingSessionsTableOperations)}: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {traceId}");
                        throw;
                    }
                });
                foreach (var session in codingSessions)
                {
                    _appLogger.Info($"Session Data: SessionID={session.sessionId}, UserID={session.userId}, startDate={session.startDate}, startTime={session.startTime}, " +
                    $"endDate={session.endDate}, endTime={session.endTime}, durationSeconds={session.durationSeconds}, durationHHMM={session.durationHHMM}, " +
                    $"goalHHMM={session.goalHHMM}, goalReached={session.goalReached}");
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
                    var dto = new CodingSessionDTO
                    {
                        sessionId = reader.IsDBNull(reader.GetOrdinal("sessionId")) ? 0 : reader.GetInt32(reader.GetOrdinal("sessionId")),
                        userId = reader.IsDBNull(reader.GetOrdinal("userId")) ? 0 : reader.GetInt32(reader.GetOrdinal("userId")),
                        startDate = reader.IsDBNull(reader.GetOrdinal("startDate")) ? null : reader.GetDateTime(reader.GetOrdinal("startDate")),
                        startTime = reader.IsDBNull(reader.GetOrdinal("startTime")) ? null : reader.GetDateTime(reader.GetOrdinal("startTime")),
                        endDate = reader.IsDBNull(reader.GetOrdinal("endDate")) ? null : reader.GetDateTime(reader.GetOrdinal("endDate")),
                        endTime = reader.IsDBNull(reader.GetOrdinal("endTime")) ? null : reader.GetDateTime(reader.GetOrdinal("endTime")),
                        durationSeconds = reader.IsDBNull(reader.GetOrdinal("durationSeconds")) ? 0 : reader.GetDouble(reader.GetOrdinal("durationSeconds")),
                        durationHHMM = reader.IsDBNull(reader.GetOrdinal("durationHHMM")) ? null : reader.GetString(reader.GetOrdinal("durationHHMM")),
                        goalHHMM = reader.IsDBNull(reader.GetOrdinal("goalHHMM")) ? null : reader.GetString(reader.GetOrdinal("goalHHMM")),
                        goalReached = reader.IsDBNull(reader.GetOrdinal("goalReached")) ? 0 : reader.GetInt32(reader.GetOrdinal("goalReached"))
                    };

                    _appLogger.Debug($"CodingSessionDTO created - sessionId: {dto.sessionId}, userId: {dto.userId}, startDate: {dto.startDate}, startTime: {dto.startTime}, " +
                                     $"endDate: {dto.endDate}, endTime: {dto.endTime}, durationSeconds: {dto.durationSeconds}, durationHHMM: {dto.durationHHMM}, " +
                                     $"goalHHMM: {dto.goalHHMM}, goalReached: {dto.goalReached}. TraceID: {activity.TraceId}");

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

