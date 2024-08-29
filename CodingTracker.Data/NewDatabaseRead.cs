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
        private readonly ICodingSession _codingSessionDTOManager;
        private readonly IUserCredentialDTOManager _userCredentialDTOManager;


        public NewDatabaseRead(IApplicationLogger appLogger, IDatabaseManager databaseManager, IQueryBuilder queryBuilder, ICodingGoalDTOManager codingGoalDTOManager, ICodingSession codingSessionDTOManager)
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
                    var dto = new UserCredentialDTO
                    {
                        UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? 0 : reader.GetInt32(reader.GetOrdinal("UserId")),
                        Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? null : reader.GetString(reader.GetOrdinal("Username")),
                        PasswordHash = reader.IsDBNull(reader.GetOrdinal("PasswordHash")) ? null : reader.GetString(reader.GetOrdinal("PasswordHash")),
                        LastLogin = reader.IsDBNull(reader.GetOrdinal("LastLogin")) ? DateTime.UtcNow : reader.GetDateTime(reader.GetOrdinal("LastLogin"))
                    };

                    _appLogger.Debug($"CodingGoalDTO created - UserId: {dto.UserId}, Username: {dto.Username}, PasswordHash: {dto.PasswordHash}, LastLogin: {dto.LastLogin}. TraceID: {activity.TraceId}");

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
                $"EndDate={endDate}, EndTime={endTime}, DurationSeconds={durationSeconds}, DurationHHMM={durationHHMM}, " +
                $"GoalHHMM={goalHHMM}, GoalReached={goalReached}, OrderBy={orderBy}, Ascending={ascending}, GroupBy={groupBy}, " +
                $"SumColumn={sumColumn}, Limit={limit}");

                try
                {
                    _databaseManager.ExecuteDatabaseOperation(connection =>
                    {
                        string commandText = _queryBuilder.CreateCommandTextForCodingSessions(columnsToSelect, sessionId, userId, startDate, startTime, endDate, endTime, durationSeconds, durationHHMM, goalHHMM, goalReached);
                        using (var command = new SQLiteCommand(commandText, connection)) // Uses the text created by CreatecommandText to generate a new command object.
                        {
                            _queryBuilder.SetCommandParametersForCodingSessions(command, sessionId, userId, startDate, startTime, endDate, endTime, durationSeconds, durationHHMM, goalHHMM, goalReached);

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
            foreach (var session in codingSessions) 
            {
                _appLogger.Info($"Session Data: SessionID={session.SessionId}, UserID={session.UserId}, StartDate={session.StartDate}, StartTime={session.StartTime}, " +
                $"EndDate={session.EndDate}, EndTime={session.EndTime}, DurationSeconds={session.DurationSeconds}, DurationHHMM={session.DurationHHMM}, " +
                $"GoalHHMM={session.GoalHHMM}, GoalReached={session.GoalReached}");
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
                        SessionId = reader.IsDBNull(reader.GetOrdinal("SessionId")) ? 0 : reader.GetInt32(reader.GetOrdinal("SessionId")),
                        UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? 0 : reader.GetInt32(reader.GetOrdinal("UserId")),
                        StartDate = reader.IsDBNull(reader.GetOrdinal("StartDate")) ? null : reader.GetDateTime(reader.GetOrdinal("StartDate")),
                        StartTime = reader.IsDBNull(reader.GetOrdinal("StartTime")) ? null : reader.GetDateTime(reader.GetOrdinal("StartTime")),
                        EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? null : reader.GetDateTime(reader.GetOrdinal("EndDate")),
                        EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? null : reader.GetDateTime(reader.GetOrdinal("EndTime")),
                        DurationSeconds = reader.IsDBNull(reader.GetOrdinal("DurationSeconds")) ? 0 : reader.GetDouble(reader.GetOrdinal("DurationSeconds")),
                        DurationHHMM = reader.IsDBNull(reader.GetOrdinal("DurationHHMM")) ? null : reader.GetString(reader.GetOrdinal("DurationHHMM")),
                        GoalHHMM = reader.IsDBNull(reader.GetOrdinal("GoalHHMM")) ? null : reader.GetString(reader.GetOrdinal("GoalHHMM")),
                        GoalReached = reader.IsDBNull(reader.GetOrdinal("GoalReached")) ? 0 : reader.GetInt32(reader.GetOrdinal("GoalReached"))
                    };

                    _appLogger.Debug($"CodingSessionDTO created - SessionId: {dto.SessionId}, UserId: {dto.UserId}, StartDate: {dto.StartDate}, StartTime: {dto.StartTime}, " +
                                     $"EndDate: {dto.EndDate}, EndTime: {dto.EndTime}, DurationSeconds: {dto.DurationSeconds}, DurationHHMM: {dto.DurationHHMM}, " +
                                     $"GoalHHMM: {dto.GoalHHMM}, GoalReached: {dto.GoalReached}. TraceID: {activity.TraceId}");

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

