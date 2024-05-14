using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.IAuthenticationServices
{
    public interface IAuthenticationService
    {
        bool AuthenticateLogin(string username, string password);
        void UpdateCurrentUserId(int UserId);
        void UpdatePasswordHash(string passwordHash);
        void UpdateLastLogin(DateTime lastLogin);


    }
}
