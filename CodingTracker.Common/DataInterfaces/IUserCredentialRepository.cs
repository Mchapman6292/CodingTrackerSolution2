using CodingTracker.Common.UserCredentials.UserCredentialDTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.DataInterfaces.IUserCredentialRepositories
{
    public interface IUserCredentialRepository
    {
        int AddUserCredential(Activity activity, UserCredentialDTO userCredential);
    }
}
