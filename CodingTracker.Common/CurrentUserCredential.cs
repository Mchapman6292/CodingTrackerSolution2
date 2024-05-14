using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.CurrentUserCredentials
{
    public class CurrentUserCredentials
    {
        private int _userId;
        private string _username;
        private string _passwordHash;
        private DateTime _lastLogin;

        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string PasswordHash
        {
            get { return _passwordHash; }
            set { _passwordHash = value; }
        }

        public DateTime LastLogin
        {
            get { return _lastLogin; }
            set { _lastLogin = value; }
        }

        public CurrentUserCredentials(int userId, string username, string passwordHash, DateTime lastLogin)
        {
            _userId = userId;
            _username = username;
            _passwordHash = passwordHash;
            _lastLogin = lastLogin;
        }
    }
}