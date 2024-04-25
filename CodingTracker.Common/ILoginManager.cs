using CodingTracker.Common.UserCredentialDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.ILoginManagers
{
    public interface ILoginManager
    {
        UserCredentialDTO ValidateLogin(string username, string password);
        void ResetPassword(string username, string newPassword);

        void AssignCurrentUserId(int? userId);

        int ReturnCurrentUserId();

    }
}