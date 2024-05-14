using CodingTracker.Common.UserCredentialDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.IUserRepositorys
{
    public interface IUserRepository
    {
        bool AuthenticateLogin(string username, string password);
        UserCredentialDTO GetUserDetails(string username);

        void ResetPassword(string username, string newPassword);

        string HashPassword(string password);

    }
}