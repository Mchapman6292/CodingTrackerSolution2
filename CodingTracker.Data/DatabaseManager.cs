using System;
using System.Data.SQLite;
using CodingTracker.Logging.IDatabaseManagers;
using CodingTracker.Data.Configurations;
using CodingTracker.Logging.IInputValidators;
using System.Data;
using CodingTracker.Logging.IStartConfiguration;

namespace CodingTracker.Data.DatabaseManagers
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly string _connectionString;
        private readonly string _databasePath;
        private readonly IInputValidator? _validator; 
        private readonly IStartConfiguration? _iStartConfiguration;
        private SQLiteConnection? _connection; // The actual connection to database using the connection string. 




        public DatabaseManager(IStartConfiguration startConfiguration, IInputValidator validator) // Provides the database path for the current user.
        {
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
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                try
                {
                    _connection = new SQLiteConnection(_iStartConfiguration.ConnectionString);
                    _connection.Open();
                    CreateTableIfNotExists();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
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