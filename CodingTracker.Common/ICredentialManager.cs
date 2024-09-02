
using System.Diagnostics;
using System.Threading.Tasks;

namespace CodingTracker.Common.ICredentialManagers
{
    public interface ICredentialManager
    {
        Task<bool> CreateAccount(Activity activity, string username, string password);


        string HashPassword(string password);



    }
}
