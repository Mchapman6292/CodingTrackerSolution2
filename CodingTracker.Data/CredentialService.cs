using System;
using CodingTracker.Common.ICredentialServices;
using CodingTracker.Common.ICredentialStorage;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.UserCredentialDTOs;


namespace CodingTracker.Data.CredentialServices
{
    public class CredentialService : ICredentialService
    {
        private readonly ICredentialStorage _credentialStorage;
        private readonly IDatabaseManager _databaseManager;
        


        public CredentialService(ICredentialStorage credentialStorage, IDatabaseManager databaseManager)
        {
            _credentialStorage = credentialStorage;
            _databaseManager = databaseManager;
        }

        public UserCredentialDTO CreateUser(string username, string password)
        {
            string hashedPassword = _credentialStorage.HashPassword(password);

            


        }



        public bool ValidateLogin(string username, string password)
        {
            if (_credentialStorage.CheckUserNameCredential(username, out UserCredentialDTO storedCredentials))
            {
                string hashedInputPassword = _credentialStorage.HashPassword(password);
                return hashedInputPassword == storedCredentials.Password;
            }
            else
            {
                return false;
            }
        }





    





    }
}
