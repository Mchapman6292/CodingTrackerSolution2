using CodingTracker.Common.UserCredentialDTOs;

namespace CodingTracker.Common.ICredentialStorage
{
    public interface ICredentialStorage
    {
        void AddCredentials(UserCredentialDTO credential);
        void UpdateCredentials(int userId, string newUsername, string newPassword);
        void UpdateUserName(int userId, string newUserName);
        void UpdatePassword(int userId, string newPassword);
        void DeleteCredentials(int userId);
        UserCredentialDTO GetCredentialById(int userId);

        string HashPassword(string password);

    }
}
