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

namespace CodingTracker.Data.DatabaseManagers
// Async methods for background operations
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly IApplicationLogger _appLogger;
        private readonly string _connectionString;
        private readonly string _databasePath;
        private readonly IInputValidator? _validator; 
        private readonly IStartConfiguration? _iStartConfiguration;
        private SQLiteConnection? _connection; // The actual connection to database using the connection string. 




        public DatabaseManager(IApplicationLogger appLogger, IStartConfiguration startConfiguration, IInputValidator validator) // Provides the database path for the current user.
        {
            _appLogger = appLogger;
            _iStartConfiguration = startConfiguration;
            _validator = validator;
            _databasePath = _iStartConfiguration.DatabasePath;
            _connectionString = _iStartConfiguration.ConnectionString;


        }

        public static string GetTodayDate() => DateTime.Today.ToString("yyyy-MM-dd");


        public void ExecuteCRUD(Action<SQLiteConnection> action)
        {
            OpenConnectionWithRetryAsync();
            action(_connection);
        }

        public async Task ExecuteCRUDAsync(Func<SQLiteConnection, Task> asyncAction)
        {
            using (var activity = new Activity(nameof(ExecuteCRUDAsync)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ExecuteCRUDAsync)}. TraceID: {activity.TraceId}");

                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    await OpenConnectionWithRetryAsync();

                    await asyncAction(_connection);

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

            public async Task OpenConnectionWithRetryAsync()
            {
                const int maxRetryCount = 3;
                int attempt = 0;
                Stopwatch overallStopwatch = Stopwatch.StartNew(); // Measures total time taken for method execution

                using (var activity = new Activity(nameof(OpenConnectionWithRetryAsync)).Start())
                {
                    _appLogger.Debug($"Starting {nameof(OpenConnectionWithRetryAsync)}. TraceID: {activity.TraceId}");

                    while (attempt < maxRetryCount)
                    {
                        Stopwatch attemptStopwatch = Stopwatch.StartNew(); // Measures individual retry attempts

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
                            await _connection.OpenAsync();
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

                            await Task.Delay(1000 * (attempt + 1)); 
                            attempt++;
                        }
                    }
                }
            }
     



            public void CreateTableIfNotExists()
                {
                    using (var command = new SQLiteCommand(_connection))
                    {
                        command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT NOT NULL,
                        CreatedAt DATETIME NOT NULL,
                        LastLogin DATETIME
                    );";

                        command.ExecuteNonQuery();


                        command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS CodingSessions (
                        SessionId INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        StartTime DATETIME NOT NULL,
                        EndTime DATETIME ,
                        StartDate DATETIME NOT NULL,
                        EndDate DATETIME ,
                        DurationMinutes INTEGER ,
                        CoadingGoalHours INTEGER,
                        TimeToGoalMinutes INTEGER,
                        SessionNotes TEXT,
                        FOREIGN KEY(UserId) REFERENCES Users(UserId)
                    );";

                        command.ExecuteNonQuery();

                    }
                }


                public bool CheckSessionIdExist(int sessionId)
                {
                    OpenConnectionWithRetryAsync();
                    using (var command = new SQLiteCommand(_connection))
                    {
                        command.CommandText = @"
                            SELECT COUNT(*) FROM CodingSessions
                            WHERE SessionId = @SessionId";
                        command.Parameters.AddWithValue("@SessionId", sessionId);

                        var result = (long)command.ExecuteScalar();
                        return result > 0;
                    }
                }
            }
        }