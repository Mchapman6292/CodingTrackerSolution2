using CodingTracker.Common.ICredentialStorageServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Data.CredentialServices
{
    public class CredentialService : ICredentialService
    {
        public CredentialService() 
        {
            
        }



        public void AddCredentials(UserCredential credential)
        {
            int userId = credential.UserId;
            string userName = credential.Username;
            string uPassword = credential.Password;

            if (_credentialsDict.ContainsKey(userId))
            {
                throw new InvalidOperationException("User Id Already exists");
            }
            else if (CheckUserName(userName))
            {
                throw new InvalidOperationException("Username already exists");
            }
            else
            {
                UpdatePassword(HashPassword(uPassword));
                _credentialsDict.Add(userId, credential);
            }
        }
        public void UpdateUserName(int userId, string newUserName)
        {
            if (!_credentialsDict.ContainsKey(userId))
            {
                throw new("User ID does not exist");
            }

            _credentialsDict[userId].Username = newUserName;
        }



        public void UpdatePassword(int userId, string newPassword)
        {
            if (!_credentialsDict.ContainsKey(userId))
            {
                throw new InvalidOperationException("User ID does not exist");
            }

            _credentialsDict[userId].Password = HashPassword(newPassword);
        }
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
}
