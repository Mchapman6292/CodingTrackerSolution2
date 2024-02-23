using CodingTracker.Common.IUserCredentialDTOs;

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
        bool CheckUserName(string username);
        bool CheckUserId(int userId);
        bool CheckUserPassword(string password);
    }
}
