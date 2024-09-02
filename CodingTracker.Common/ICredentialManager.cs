using CodingTracker.Common.UserCredentialDTOs;

namespace CodingTracker.Common.ICredentialManagers
{
    public interface ICredentialManager
    {
        Task CreateAccount(string username, string password);

        UserCredentialDTO GetCredentialById(int userId);

        string HashPassword(string password);

        bool IsAccountCreatedSuccessfully(string username);


    }
}
