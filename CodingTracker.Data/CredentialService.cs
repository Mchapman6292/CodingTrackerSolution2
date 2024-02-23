using System;
using CodingTracker.Common.ICredentialServices;
using CodingTracker.Common.ICredentialStorage;
using CodingTracker.Data.UserCredentialDTOs;
using System.Text;


namespace CodingTracker.Data.CredentialServices
{
    public class CredentialService : ICredentialService
    {
        private readonly ICredentialStorage _credentialStorage;


        public CredentialService(ICredentialStorage credentialStorage)
        {
            _credentialStorage = credentialStorage;
        }


        public bool ValidateLogin(string username, string password)
        {
            if (_credentialStorage.)
        }








        public bool ValidateLogin(string username, string password)
        {
            if (_credentialsDict.TryGetValue(username, out UserCredentialDTO storedCredentials))
            {
                string hashedInputPassword = HashPassword(password);
                return hashedInputPassword == storedCredentials.Password;
            }
            else
            {
                // Username not found
                return false;
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
    }
}
