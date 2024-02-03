using CodingTracker.UserCredentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UCredentials = CodingTracker.UserCredentials.UserCredential;
using System.Security.Cryptography;

//

namespace CodingTracker.Data.CredentialStorages
{
    public class CredentialStorage
    {
        private Dictionary<int, UserCredential> _credentialsDict;
        private readonly UCredentials _UCredentials;

        public CredentialStorage(UCredentials uCredentials)
        {
            _credentialsDict = new Dictionary<int, UserCredential>();
            _UCredentials = uCredentials;
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


        public bool CheckUserName(string username)
        {
            return _credentialsDict.Values.Any(credential => credential.Username == username);
        }

        public bool CheckUserId(int userId)
        {
            return _credentialsDict.ContainsKey(userId);
        }

        public bool CheckUserPassword(string password)
        {
            return _credentialsDict.Values.Any(credential => credential.Password == password);
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


    