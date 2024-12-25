
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.IAuthenticationServices
{
    public interface IAuthenticationService
    {
        Task<bool> CreateAccount(string username, string password);
        Task<bool> AuthenticateLogin(string username, string password, Activity activity);

        Task<bool> ResetPassword(string username, string newPassword);


    }
}