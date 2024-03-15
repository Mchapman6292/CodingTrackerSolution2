using System;
using System.Data.SQLite;
using System.Threading.Tasks;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Data.Configurations;
using CodingTracker.Common.IInputValidators;
using System.Data;
using CodingTracker.Common.IStartConfigurations;
using CodingTracker.Common.IApplicationLoggers;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;

namespace CodingTracker.Data.DatabaseManagers
// Async methods for background operations
{
    public class DatabaseManager : IDatabaseManager
    {
        private static SQLiteConnection? _connection; // The actual connection to database using the connection string. 
        private readonly IApplicationLogger _appLogger;
        private readonly string _connectionString; // Creates an open instance of SQLiteConnection which is then stored in _connection.
        private readonly string _databasePath;
        private readonly IStartConfiguration? _iStartConfiguration;





        public DatabaseManager(IApplicationLogger appLogger, IStartConfiguration startConfiguration, IInputValidator validator) // Provides the database path for the current user.
        {
            _appLogger = appLogger;
            _iStartConfiguration = startConfiguration;
            _databasePath = _iStartConfiguration.DatabasePath;
            _connectionString = _iStartConfiguration.ConnectionString;


        }

        public static string GetTodayDate() => DateTime.Today.ToString("yyyy-MM-dd");



        public void OpenConnectionWithRetry()
        {
            const int maxRetryCount = 3;
            int attempt = 0;
            Stopwatch overallStopwatch = Stopwatch.StartNew();

            using (var activity = new Activity(nameof(OpenConnectionWithRetry)).Start())
            {
                _appLogger.Debug($"Starting {nameof(OpenConnectionWithRetry)}. TraceID: {activity.TraceId}");

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
            throw new NotImplementedException();
        }


        public void ExecuteCRUD(Action<SQLiteConnection> action)
        {
            using (var activity = new Activity(nameof(ExecuteCRUD)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ExecuteCRUD)}. TraceID: {activity.TraceId}");

                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    OpenConnectionWithRetry();

                    action(_connection);

                    stopwatch.Stop();
                    _appLogger.Info($"Executed CRUD operation successfully. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to execute CRUD operation. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    throw;
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
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(EnsureDatabaseConnection)).Start())
            {
                _appLogger.Debug($"Starting {nameof(EnsureDatabaseConnection)}. TraceID: {activity.TraceId}");

                try
                {
                    if (_connection == null || _connection.State != ConnectionState.Open)
                    {
                        OpenConnectionWithRetry();
                    }
                    stopwatch.Stop();
                    _appLogger.Info($"Database connection ensured successfully. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to ensure database connection. Error: {ex.Message}. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    throw;
                }
            }
        }



        public void CreateTableIfNotExists()
        {
            using (var activity = new Activity(nameof(CreateTableIfNotExists)).Start())
            {
                _appLogger.Info($"Starting {nameof(CreateTableIfNotExists)}. TraceID: {activity.TraceId}.");

                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    using var command = _connection.CreateCommand();

                    command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    CreatedAt DATETIME NOT NULL,
                    LastLogin DATETIME
                );

                CREATE TABLE IF NOT EXISTS CodingSessions (
                    SessionId INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    StartTime DATETIME NOT NULL,
                    EndTime DATETIME,
                    StartDate DATETIME NOT NULL,
                    EndDate DATETIME,
                    DurationMinutes INTEGER,
                    CodingGoalHours INTEGER,
                    TimeToGoalMinutes INTEGER,
                    SessionNotes TEXT,
                    FOREIGN KEY(UserId) REFERENCES Users(UserId)
                );";

                    int affectedRows = command.ExecuteNonQuery();

                    stopwatch.Stop();
                    _appLogger.Info($"Tables created/verified successfully. Rows affected: {affectedRows}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (SQLiteException ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to create/verify tables. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                }
            }
        }


        public bool CheckSessionIdExist(int sessionId)
        {
            using (var activity = new Activity(nameof(CheckSessionIdExist)).Start())
            {
                _appLogger.Debug($"Starting {nameof(CheckSessionIdExist)}. TraceID: {activity.TraceId}, SessionId: {sessionId}");

                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    OpenConnectionWithRetry();
                    using var command = _connection.CreateCommand();

                    command.CommandText = @"
                SELECT COUNT(*) FROM CodingSessions
                WHERE SessionId = @SessionId";
                    command.Parameters.AddWithValue("@SessionId", sessionId);

                    var result = (long)command.ExecuteScalar();

                    stopwatch.Stop();
                    _appLogger.Info($"Session ID check completed. SessionId: {sessionId}. Exists: {result > 0}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");

                    return result > 0;
                }
                catch (SQLiteException ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to check session ID. SessionId: {sessionId}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    return false;

                }
            }
        }
    }
}