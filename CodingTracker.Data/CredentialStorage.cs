
using System.Text;
using System.Security.Cryptography;
using CodingTracker.Data.UserCredentialDTOs;
using CodingTracker.Common.ICredentialStorage;
using CodingTracker.Data.CredentialServices;
using CodingTracker.Common.ICredentialServices;


//

namespace CodingTracker.Data.CredentialStorage
{
    public class CredentialStorage : ICredentialStorage
    {
        private readonly UserCredentialDTO _uCredentials;
        private Dictionary<int, UserCredentialDTO> _credentialsById;
        private readonly ICredentialService _credentialService;

        public CredentialStorage( ICredentialService credentialService)
        {
            _credentialsById = new Dictionary<int, UserCredentialDTO>();
            _credentialService = credentialService;
        }


        public void AddCredentials(UserCredentialDTO credential)
        {
            int userId = credential.UserId;
            string userName = credential.Username;
            string uPassword = credential.Password;

            if (_credentialsById.ContainsKey(userId))
            {
                throw new InvalidOperationException("User Id Already exists");
            }
            credential.Password = HashPassword(credential.Password);
            _credentialsById.Add(credential.UserId, credential);
        }

        public void UpdateCredentials(int userId, string newUsername, string newPassword)
        {
            if (!CheckUserId(userId))
            {
                throw new InvalidOperationException("User ID does not exist");
            }

            if (!string.IsNullOrEmpty(newUsername) && !CheckUserName(newUsername))
            {
                UpdateUserName(userId, newUsername);
            }
            if (!string.IsNullOrEmpty(newPassword))
            {
                UpdatePassword(userId, newPassword);
            }
        }


        public void UpdateUserName(int userId, string newUserName)
        {
            if (!_credentialsById.ContainsKey(userId))
            {
                throw new("User ID does not exist");
            }

            _credentialsById[userId].Username = newUserName;
        }



        public void UpdatePassword(int userId, string newPassword)
        {
            if (!_credentialsById.ContainsKey(userId))
            {
                throw new InvalidOperationException("User ID does not exist");
            }

            _credentialsById[userId].Password = HashPassword(newPassword);
        }

        public void DeleteCredentials(int userId)
        {
            if (!_credentialsById.ContainsKey(userId))
            {
                throw new InvalidOperationException("User ID does not exist");
            }
            _credentialsById.Remove(userId);
        }

        public UserCredentialDTO GetCredentialById(int userId)
        {
            if (_credentialsById.TryGetValue(userId, out UserCredentialDTO credential))
            {
                return credential;
            }
            throw new KeyNotFoundException($"No credentials found for userId {userId}");
        }



        public static string HashPassword(string password)
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


        public bool CheckUserName(string username)
        {
            return _credentialsById.Values.Any(credential => credential.Username == username);
        }


        public bool CheckUserId(int userId)
        {
            return _credentialsById.ContainsKey(userId);
        }

        public bool CheckUserPassword(string password)
        {
            return _credentialsById.Values.Any(credential => credential.Password == password);
        }
    }

}

}



    