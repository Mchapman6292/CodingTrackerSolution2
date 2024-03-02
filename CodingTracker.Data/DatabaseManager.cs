using System;
using System.Data.SQLite;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Data.Configurations;
using CodingTracker.Common.IInputValidators;
using System.Data;
using CodingTracker.Common.IStartConfigurations;
using CodingTracker.Common.IApplicationLoggers;
using System.Data.SqlClient;
using System.Diagnostics;

namespace CodingTracker.Data.DatabaseManagers
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
            OpenConnection();
            action(_connection);
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




        public void OpenConnection()
        {
            using (var activity = new Activity(nameof(OpenConnection)).Start())
            {
                _appLogger.Debug($"Starting {nameof(OpenConnection)}. TraceID: {activity.TraceId}");

                try
                {
                    if (_connection != null && _connection.State == ConnectionState.Open)
                    {
                        _appLogger.Debug($"Connection already open. TraceID: {activity.TraceId}");
                        return;
                    }

                    _connection = new SQLiteConnection(_connectionString);
                    _connection.Open();
                    _appLogger.Info($"Connection opened successfully. TraceID: {activity.TraceId}");
                }
                catch (SqlException ex)
                {
                    _appLogger.Error($"SQL error occurred while opening connection: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    throw;
                }
                catch (InvalidOperationException ex)
                {
                    _appLogger.Error($"Invalid operation while opening connection: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    throw;
                }
                catch (TimeoutException ex)
                {
                    _appLogger.Error($"Timeout occurred while opening connection: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    throw;
                }
                // Include additional catch blocks for other specific exceptions as needed
                catch (Exception ex)
                {
                    _appLogger.Error($"Unexpected error while opening connection: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    throw;
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
            OpenConnection();
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