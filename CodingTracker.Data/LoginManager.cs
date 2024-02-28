using CodingTracker.Common.ILoginManagers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.ICredentialStorage;
using System.Data.SQLite;
using System.Data.SqlClient;
using CodingTracker.Common.UserCredentialDTOs;


// resetPassword, updatePassword, rememberUser 
namespace CodingTracker.Data.LoginManagers
{
    public class LoginManager : ILoginManager
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly ICredentialStorage _credentialStorage;
        public LoginManager(IDatabaseManager databaseManager, ICredentialStorage credentialStorage) 
        {
            _databaseManager = databaseManager;
            _credentialStorage = credentialStorage;
        }


        public UserCredentialDTO ValidateLogin(string username, string password)
        {
            try
            {
                var hashedPassword = _credentialStorage.HashPassword(password);

                UserCredentialDTO userCredential = null; // Initialize as null to indicate no user found by default

                _databaseManager.ExecuteCRUD(connection =>
                {
                    using var command = connection.CreateCommand();
                    command.CommandText = @"
                SELECT 
                        UserId, Username, PasswordHash 
                FROM 
                        UserCredentials 
                WHERE 
                        Username = @Username";
                    command.Parameters.AddWithValue("@Username", username);

                    using var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        var storedHash = reader["PasswordHash"].ToString();
                        if (hashedPassword == storedHash)
                        {
                            // Construct the DTO with data from the database
                            userCredential = new UserCredentialDTO
                            {
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Username = reader["Username"].ToString(),
                                Password = password
                            };
                           
                        }
                    }
                });

                return userCredential; // Returns null if no matching user, or the UserCredentialDTO if a match is found
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null; // Indicate failure by returning null
            }
        }
    



    public void ResetPassword(string username, string newPassword)
        {

            string hashedPassword = _credentialStorage.HashPassword(newPassword);

            _databaseManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE 
                        Users
                    SET 
                        PasswordHash = @HashedPassword
                    WHERE
                        Username = @Username";
                command.Parameters.AddWithValue("@HashedPassword", hashedPassword);
                command.Parameters.AddWithValue("@Username", username);

                try
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("No user found with the specified username. Password reset failed.");
                    }
                    else
                    {
                        Console.WriteLine("Password has been successfully reset.");
                    }
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine($"An error occurred during password reset: {ex.Message}");
                }
            });
        }




    }
}
