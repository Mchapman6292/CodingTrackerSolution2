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

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                string commandText = _queryBuilder.CreateCommandTextForUserCredentials(userId, username, passwordHash, lastLoginDate,  orderBy, ascending, groupBy, limit);    
                using (var command = new SQLiteCommand(commandText, connection))
                {
                    _queryBuilder.SetCommandParametersForUserCredentials(command);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userCredentials.Add(ExtractUserCredentialFromReader(reader));
                        }
                    }
                }
            }, nameof(ReadFromUserCredentialsTable));

            return userCredentials;
        }

        private UserCredentialDTO ExtractUserCredentialFromReader(SQLiteDataReader reader)
        {
            return new UserCredentialDTO
            {
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                LastLogin = reader.IsDBNull(reader.GetOrdinal("LastLogin")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("LastLogin"))
            };
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

                try
                {
                    _databaseManager.ExecuteDatabaseOperation(connection =>
                    {
                        string commandText = _queryBuilder.CreateCommandTextForCodingSessions(sessionId, userId, startDate, startTime, endDate, endTime, false, orderBy, ascending, groupBy, sumColumn, limit);
                        using (var command = new SQLiteCommand(commandText, connection))
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
            return new CodingSessionDTO
            {
                SessionId = reader.GetInt32(reader.GetOrdinal("SessionId")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? null : reader.GetDateTime(reader.GetOrdinal("EndDate")),
                EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? null : reader.GetDateTime(reader.GetOrdinal("EndTime")),
                DurationSeconds = reader.IsDBNull(reader.GetOrdinal("DurationSeconds")) ? null : (double?)reader.GetDouble(reader.GetOrdinal("DurationSeconds")),
                DurationHHMM = reader.IsDBNull(reader.GetOrdinal("DurationHHMM")) ? null : reader.GetString(reader.GetOrdinal("DurationHHMM")),
                GoalHHMM = reader.IsDBNull(reader.GetOrdinal("GoalHHMM")) ? null : reader.GetString(reader.GetOrdinal("GoalHHMM")),
                GoalReached = reader.IsDBNull(reader.GetOrdinal("GoalReached")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("GoalReached"))
            };
        }

    }
}

