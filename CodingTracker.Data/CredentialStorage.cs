
using System.Text;
using System.Security.Cryptography;
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.ICredentialStorage;
using CodingTracker.Data.CredentialServices;
using CodingTracker.Common.ICredentialServices;
using CodingTracker.Common.IDatabaseManagers;
using System.Data.SQLite;


//

namespace CodingTracker.Data.CredentialStorage
{
    public class CredentialStorage : ICredentialStorage
    {
        private readonly UserCredentialDTO _uCredentials;
        private readonly IDatabaseManager _databaseManager;
        private Dictionary<string, UserCredentialDTO> _credentialsDict = new Dictionary<string, UserCredentialDTO>();
        private readonly ICredentialService _credentialService;

        public CredentialStorage( ICredentialService credentialService, IDatabaseManager databaseManager)
        {
            _credentialsDict = new Dictionary<string, UserCredentialDTO>();
            _credentialService = credentialService;
            _databaseManager = databaseManager;
        }


        public void AddCredentials(UserCredentialDTO credential)
        {
            string hashedPassword = HashPassword(credential.Password);
            _databaseManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
                INSERT INTO 
                        UserCredentials
                (
                        UserId,
                        Username,
                        PasswordHash
                )
                VALUES
                (
                        @UserId,
                        @Username,
                        @PasswordHash
                )";
                command.Parameters.AddWithValue("@UserId", credential.UserId);
                command.Parameters.AddWithValue("@Username", credential.Username);
                command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

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

        public void UpdateCredentials(int userId, string newUsername, string newPassword)
        {
            UpdateUserName(userId, newUsername);
            UpdatePassword(userId, newPassword);
        }



        public void UpdateUserName(int userId, string newUserName)
        {
            _databaseManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
                UPDATE 
                        UserCredentials
                SET 
                        Username = @Username
                WHERE 
                        UserId = @UserId";
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Username", newUserName);

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


        public void UpdatePassword(int userId, string newPassword)
        {
            string hashedPassword = HashPassword(newPassword);
            _databaseManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
                UPDATE 
                        UserCredentials
                SET 
                        PasswordHash = @PasswordHash
                WHERE 
                        UserId = @UserId";
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

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

        public void DeleteCredentials(int userId)
        {
            _databaseManager.ExecuteCRUD(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
                DELETE FROM 
                        UserCredentials
                WHERE 
                        UserId = @UserId";
                command.Parameters.AddWithValue("@UserId", userId);

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
    

        public UserCredentialDTO GetCredentialById(int userId) // Needed?
        {
            if (_credentialsDict.TryGetValue(userId, out UserCredentialDTO credential))
            {
                return credential;
            }
            throw new KeyNotFoundException($"No credentials found for userId {userId}");
        }



        public  string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {

                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }


        public bool CheckUserNameCredential(string username, out UserCredentialDTO userCredential) // Probably not needed
        {
            return _credentialsDict.TryGetValue(username, out userCredential);
        }


        public bool CheckUserIdCredentialCredential(int userId)
        {
            return _credentialsDict.ContainsKey(userId);
        }

        public bool CheckUserPasswordCredential(string password)
        {
            return _credentialsDict.Values.Any(credential => credential.Password == password);
        }
    }

}




    