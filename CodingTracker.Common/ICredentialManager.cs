using CodingTracker.Common.UserCredentialDTOs;

namespace CodingTracker.Common.ICredentialManagers
{
    public interface ICredentialManager
    {
        void CreateAccount(string username, string password);
        void UpdateUserName(int userId, string newUserName);
        void UpdatePassword(int userId, string newPassword);
        void DeleteCredentials(int userId);
        UserCredentialDTO GetCredentialById(int userId);

        string HashPassword(string password);

    }
}
