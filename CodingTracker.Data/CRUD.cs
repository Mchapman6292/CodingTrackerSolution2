using System.Data.SQLite;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.ICRUDs;
using CodingTracker.Common.CodingSessionDTOs;



namespace CodingTracker.Data.CRUDs
{
    public class CRUD : ICRUD
    {
        private readonly IDatabaseManager _dbManager;
        private readonly CodingSessionDTO _codingSessionDTO;



        public CRUD(IDatabaseManager databaseManager, CodingSessionDTO codingSessionDTO)
        {
            _dbManager = databaseManager;
            _codingSessionDTO = codingSessionDTO;
        }

        public void UpdateSession()
        {
            _dbManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
            UPDATE CodingSessions  
            SET 
                StartTime = @StartTime, 
                EndTime = @EndTime, 
                StartDate = @StartDate,
                EndDate = @EndDate,
                DurationMinutes = @Duration, 
                SessionNotes = @SessionNotes
            WHERE 
                SessionId = @SessionId AND 
                UserId = @UserId";

                command.Parameters.AddWithValue("@SessionId", _codingSessionDTO.SessionId);
                command.Parameters.AddWithValue("@UserId", _codingSessionDTO.UserId);
                command.Parameters.AddWithValue("@StartTime", _codingSessionDTO.StartTime);
                command.Parameters.AddWithValue("@EndTime", _codingSessionDTO.EndTime.HasValue ? (object)_codingSessionDTO.EndTime.Value : DBNull.Value);
                command.Parameters.AddWithValue("@StartDate", _codingSessionDTO.StartDate.HasValue ? _codingSessionDTO.StartDate.Value.ToString("yyyy-MM-dd") : DBNull.Value);
                command.Parameters.AddWithValue("@EndDate", _codingSessionDTO.EndDate.HasValue ? _codingSessionDTO.EndDate.Value.ToString("yyyy-MM-dd") : DBNull.Value);
                command.Parameters.AddWithValue("@Duration", _codingSessionDTO.Duration.HasValue ? (object)_codingSessionDTO.Duration.Value.TotalMinutes : DBNull.Value);
                command.Parameters.AddWithValue("@SessionNotes", _codingSessionDTO.SessionNotes != null ? (object)_codingSessionDTO.SessionNotes : DBNull.Value);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }


        public void DeleteSession()
        {
            _dbManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
            DELETE FROM 
                CodingSessions 
            WHERE 
                SessionId = @SessionId AND 
                UserId = @UserId";

                command.Parameters.AddWithValue("@SessionId", _codingSessionDTO.SessionId);
                command.Parameters.AddWithValue("@UserId", _codingSessionDTO.UserId);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }


        public void InsertSession()
        {
            _dbManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
            INSERT INTO CodingSessions 
            (
                SessionId, 
                UserId, 
                StartTime, 
                EndTime, 
                StartDate,
                EndDate,
                DurationMinutes, 
                SessionNotes
            ) 
            VALUES 
            (
                @SessionId, 
                @UserId, 
                @StartTime, 
                @EndTime, 
                @StartDate,
                @EndDate,
                @Duration, 
                @SessionNotes
            )";

                command.Parameters.AddWithValue("@SessionId", _codingSessionDTO.SessionId);
                command.Parameters.AddWithValue("@UserId", _codingSessionDTO.UserId);
                command.Parameters.AddWithValue("@StartTime", _codingSessionDTO.StartTime);
                command.Parameters.AddWithValue("@EndTime", _codingSessionDTO.EndTime.HasValue ? (object)_codingSessionDTO.EndTime.Value : DBNull.Value);
                command.Parameters.AddWithValue("@StartDate", _codingSessionDTO.StartDate.HasValue ? _codingSessionDTO.StartDate.Value.ToString("yyyy-MM-dd") : DBNull.Value);
                command.Parameters.AddWithValue("@EndDate", _codingSessionDTO.EndDate.HasValue ? _codingSessionDTO.EndDate.Value.ToString("yyyy-MM-dd") : DBNull.Value);
                command.Parameters.AddWithValue("@Duration", _codingSessionDTO.Duration.HasValue ? (object)_codingSessionDTO.Duration.Value.TotalMinutes : DBNull.Value);
                command.Parameters.AddWithValue("@SessionNotes", _codingSessionDTO.SessionNotes != null ? (object)_codingSessionDTO.SessionNotes : DBNull.Value);

                command.ExecuteNonQuery();
            });
        }


        public void ViewRecentSession(int numberOfSessions)
        {
            _dbManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT * FROM
                        CodingSessions
                    WHERE 
                        UserId = @userId
                    ORDER BY
                        Date DESC, StartTime DESC
                    LIMIT
                        @NumberOfSessions";


                command.Parameters.AddWithValue("@UserId", _codingSessionDTO.UserId);
                command.Parameters.AddWithValue("@NumberOfSessions", numberOfSessions);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }

        public void ViewAllSession(bool partialView = false)
        {
            var partialColumns = "SessionId, StartDate, EndDate";

            _dbManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();

                if (!partialView)
                {
                    command.CommandText = @"
                        SELECT * FROM
                            CodingSessions
                        WHERE
                            UserId = @userId
                        ORDER BY
                            Date DESC, StartTime DESC";
                }
                else
                {
                    command.CommandText = $@"
                        SELECT {partialColumns} FROM
                            CodingSessions
                        WHERE
                            UserId = @UserId
                        ORDER BY
                            StartDate DESC, StartTime DESC";
                }
                command.Parameters.AddWithValue("@UserId", _codingSessionDTO.UserId);
                try
                {
                    command.ExecuteNonQuery();

                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }


        public void ViewSpecific(string chosenDate)
        {
            _dbManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();

                command.CommandText = @"
                    SELECT SessionId, StartTime, EndTime FROM 
                        CodingSessions 
                    WHERE
                        UserId = @UserId AND Date = @Date
                    ORDER BY
                        StartTime DESC";

                command.Parameters.AddWithValue("@UserId", _codingSessionDTO.UserId);
                command.Parameters.AddWithValue("@Date", chosenDate.ToString("yyyy-MM-dd"));

                try
                {
                    command.ExecuteNonQuery();

                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }

        public void FilterSessionsByDay(string date, bool isDescending)
        {
            _dbManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();

                string order = isDescending ? "DESC" : "ASC";
                command.CommandText = $@"
                    SELECT * FROM
                        CodingSessions 
                    WHERE
                        DATE(StartTime) = DATE(@Date)
                    ORDER BY
                        StartTime {order}";

                command.Parameters.AddWithValue("@Date", date);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }

        public void FilterSessionsByWeek(string date, bool isDescending)
        {
            _dbManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();

                string order = isDescending ? "DESC" : "ASC";
                command.CommandText = $@"
                    SELECT * FROM 
                        CodingSessions 
                    WHERE strftime('%W', StartTime) = strftime('%W', @Date) AND 
                          strftime('%Y', StartTime) = strftime('%Y', @Date) 
                    ORDER BY StartTime {order}";

                command.Parameters.AddWithValue("@Date", date);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }

        public void FilterSessionsByYear(string year, bool isDescending)
        {
            _dbManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();

                string order = isDescending ? "DESC" : "ASC";
                command.CommandText = $@"
                    SELECT * FROM
                        CodingSessions 
                    WHERE
                        strftime('%Y', StartTime) = @Year 
                    ORDER BY
                        StartTime {order}";

                command.Parameters.AddWithValue("@Year", year);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }

        public void SetProgressGoal(int ProgressGoal)
        {
            _dbManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();
            }


    }
}