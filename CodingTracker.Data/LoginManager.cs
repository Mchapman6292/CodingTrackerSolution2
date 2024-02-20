using CodingTracker.Common.ILoginManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Data.LoginManagers
{
    public class LoginManager : ILoginManager
    {

        public LoginManager() 
        {


        }

        public bool ValidateLoginCredentials(string username, string password)
        {
            string hashedPassword = HashPassword(password);
            return _credentialsDict.Values.Any(credential =>
                credential.Username == username && credential.Password == hashedPassword);
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
