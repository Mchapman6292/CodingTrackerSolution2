using CodingTracker.Logging.ICredentialStorage;
using CodingTracker.Logging.IDatabaseManagers;
using CodingTracker.Logging.UserCredentialDTOs;
using System.Data.SQLite;

// Logic that includes CRUD methods to update database
namespace CodingTracker.Data.UserManagement
{
    public class UserManagement
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly ICredentialStorage _credentialStorage;


        public UserManagement(IDatabaseManager databaseManager, ICredentialStorage credentialStorage)
        {
            _databaseManager = databaseManager;
            _credentialStorage = credentialStorage;
        }

        public void AddUser(string username, string password)
        {
            string hashedPassword = _credentialStorage.HashPassword(password);
            _databaseManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
            INSERT INTO 
                    Users
            (
                    Username,
                    PasswordHash
            )
            VALUES
            (
                    @username,
                    @hashedPassword
            )";

                command.Parameters.AddWithValue("Username", username);
                command.Parameters.AddWithValue("PasswordHash", hashedPassword);

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


        public void DeleteUser(string username)
        {
            _databaseManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
            DELETE FROM 
                     Users
            WHERE
                     Username = @Username";
                command.Parameters.AddWithValue("@Username", username);

                try
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("No user found with the specified username.");
                    }
                    else
                    {
                        Console.WriteLine("User successfully deleted.");
                    }
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }

    }
}

