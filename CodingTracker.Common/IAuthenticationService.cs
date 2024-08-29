using CodingTracker.Common.UserCredentialDTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.ILoginManagers
{
    public interface IAuthenticationService
    {
        Task<bool> AuthenticateLogin(string username, string password, Activity activity);
        UserCredentialDTO GetUserDetails(string username);

        void ResetPassword(string username, string newPassword);

        string HashPassword(string password, Activity activity);

    }
}