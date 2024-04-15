using System;
using System.Data.SQLite;
using System.Threading.Tasks;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Data.Configurations;
using CodingTracker.Common.IInputValidators;
using System.Data;
using CodingTracker.Common.IStartConfigurations;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IErrorHandlers;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Data.Common;

namespace CodingTracker.Data.DatabaseManagers
{
    public class DatabaseManager : IDatabaseManager
    {
        private static SQLiteConnection? _connection; // The actual connection to database using the connection string. 
        private readonly IApplicationLogger _appLogger;
        private readonly string _connectionString; // Creates an open instance of SQLiteConnection which is then stored in _connection.
        private readonly string _databasePath;
        private readonly IStartConfiguration? _iStartConfiguration;
        private readonly IErrorHandler _errorHandler;






        public DatabaseManager(IApplicationLogger appLogger, IStartConfiguration startConfiguration, IInputValidator validator, IErrorHandler errorHandler) // Provides the database path for the current user.
        {
            _appLogger = appLogger;
            _iStartConfiguration = startConfiguration;
            _databasePath = _iStartConfiguration.DatabasePath;
            _connectionString = _iStartConfiguration.ConnectionString;
            _errorHandler = errorHandler;
            
        }

        public static string GetTodayDate() => DateTime.Today.ToString("yyyy-MM-dd");



        public void OpenConnectionWithRetry()
        {
            const int maxRetryCount = 3;
            int attempt = 0;
            Stopwatch overallStopwatch = Stopwatch.StartNew();

            using (var activity = new Activity(nameof(OpenConnectionWithRetry)).Start())
            {
                _appLogger.Debug($"Starting {nameof(OpenConnectionWithRetry)} Using connection string: {_connectionString}. TraceID: {activity.TraceId}");

                while (attempt < maxRetryCount)
                {
                    Stopwatch attemptStopwatch = Stopwatch.StartNew();

                    try
                    {
                        if (_connection != null && _connection.State == ConnectionState.Open)
                        {
                            attemptStopwatch.Stop();
                            overallStopwatch.Stop();
                            _appLogger.Debug($"Connection already open. Attempt Duration: {attemptStopwatch.ElapsedMilliseconds}ms. Total Duration: {overallStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                            return;
                        }

                        _connection = new SQLiteConnection(_connectionString);
                        _connection.Open();
                        attemptStopwatch.Stop();
                        overallStopwatch.Stop();
                        _appLogger.Info($"Connection opened successfully on attempt {attempt + 1}. Attempt Duration: {attemptStopwatch.ElapsedMilliseconds}ms. Total Duration: {overallStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                        return;
                    }
                    catch (Exception ex) when (ex is SQLiteException || ex is TimeoutException)
                    {
                        attemptStopwatch.Stop();
                        _appLogger.Warning($"Attempt {attempt + 1} failed: {ex.Message}. Attempt Duration: {attemptStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");

                        if (attempt == maxRetryCount - 1)
                        {
                            overallStopwatch.Stop();
                            _appLogger.Error($"All attempts to open the connection failed. Last error: {ex.Message}. Total Duration: {overallStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                            throw;
                        }

                        Thread.Sleep(1000 * (attempt + 1));
                        attempt++;
                    }
                }
            }
        }

        public void CloseDatabaseConnection()
        {
            using (var activity = new Activity(nameof(CloseDatabaseConnection)).Start())
            {
                _appLogger.Debug($"Starting {nameof(CloseDatabaseConnection)}. TraceID: {activity.TraceId}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    if(_connection != null && _connection.State != ConnectionState.Closed)
                    {
                        _connection.Close();
                        _appLogger.Info($"Database connection closed successfully. TraceID: {activity.TraceId}");
                    }
                    else
                    {
                        _appLogger.Info($"Database connection was already closed. TraceID: {activity.TraceId}");
                    }
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"Failed to close database connection. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    throw;
                }
            }
        }


        public void ExecuteCRUD(Action<SQLiteConnection> action)
        {
            OpenConnectionWithRetry();
            action(_connection);
        }

        public void ExecuteDatabaseOperation(Action<SQLiteConnection> operation, string operationName)
        {
            using (var activity = new Activity(operationName).Start())
            {
                _appLogger.Debug($"Starting {operationName}. TraceID: {activity.TraceId}");

                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    ExecuteCRUD(connection =>
                    {
                        operation(connection);
                        stopwatch.Stop();
                        _appLogger.Info($"{operationName} executed successfully. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    });
                }
                catch (SQLiteException ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"SQLite error in {operationName}. SQLite error code: {ex.ErrorCode}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");

                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to execute {operationName}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");

                }
            }
        }





        public void EnsureDatabaseForUser()
        {
            if (_connection == null || _connection.ConnectionString != $"Data Source={_databasePath};Version=3;")
            {
                _connection?.Close();
                _connection = new SQLiteConnection($"Data Source={_databasePath};Version=3;");
                _connection.Open();
                CreateTableIfNotExists();
            }
        }


        private void EnsureDatabaseConnection()
        {
            _errorHandler.CatchErrorsAndLogWithStopwatch(() =>
            {
                if (_connection == null || _connection.State != ConnectionState.Open)
                {
                    OpenConnectionWithRetry();
                }
            }, nameof(EnsureDatabaseConnection), true);
        }


        public void CreateTableIfNotExists()
        {
            using var command = _connection.CreateCommand();


            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS UserCredentials (
                    UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    LastLogin DATETIME
                );

                CREATE TABLE IF NOT EXISTS CodingSessions (
                    SessionId INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    StartTime DATETIME NOT NULL,
                    EndTime DATETIME,
                    DurationSeconds INTEGER,
                    DurationHHMM STRING,
                    GoalHHMM STRING,
                    GoalReached INTEGER,
                    FOREIGN KEY(UserId) REFERENCES UserCredentials(UserId)
                );";

            command.ExecuteNonQuery();
        }

        public void UpdateCodingSessionsTable()
        {
            using (var activity = new Activity(nameof(UpdateCodingSessionsTable)).Start())
            {
                _appLogger.Debug($"Starting {nameof(UpdateCodingSessionsTable)}. TraceID: {activity.TraceId}");

                OpenConnectionWithRetry();

                using var command = _connection.CreateCommand();

                command.CommandText = "DROP TABLE IF EXISTS CodingSessions;";
                command.ExecuteNonQuery();

 
                command.CommandText = @"
            CREATE TABLE CodingSessions (
                SessionId INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                StartTime DATETIME NOT NULL,
                EndTime DATETIME,
                DurationSeconds  INTEGER,
                DurationHHMM STRING,
                GoalHHMM STRING,
                GoalReached INTEGER,
                FOREIGN KEY(UserId) REFERENCES UserCredentials(UserId)
            );";
                command.ExecuteNonQuery();

                _appLogger.Info($"Updated CodingSessions table successfully. TraceID: {activity.TraceId}");
            }
        }




        public void UpdateUserCredentialsTable()
        {
            using (var activity = new Activity(nameof(UpdateUserCredentialsTable)).Start())
            {
                _appLogger.Debug($"Starting {nameof(UpdateUserCredentialsTable)}. TraceID: {activity.TraceId}");

                OpenConnectionWithRetry();

                using var command = _connection.CreateCommand();
                command.CommandText = "DROP TABLE IF EXISTS UserCredentials;";
                command.ExecuteNonQuery();

                command.CommandText = @"
                    CREATE TABLE UserCredentials (
                        UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT NOT NULL,
                        LastLogin DATETIME 
                    );";
                command.ExecuteNonQuery();

                _appLogger.Info($"Updated UserCredentials table successfully. TraceID: {activity.TraceId}");
            }
        }
        

        public bool CheckSessionIdExist(int sessionId) // Needed?
        {
            OpenConnectionWithRetry();
            using var command = _connection.CreateCommand();
            command.CommandText = @"
                SELECT
                        COUNT(*)
                FROM
                        CodingSessions
                WHERE
                        SessionId = @SessionId";
            command.Parameters.AddWithValue("@SessionId", sessionId);

            var result = (long)command.ExecuteScalar();
            return result > 0;
        }
    }
}