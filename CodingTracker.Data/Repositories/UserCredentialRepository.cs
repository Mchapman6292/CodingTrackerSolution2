using CodingTracker.Common.DataInterfaces.IUserCredentialRepositories;
using CodingTracker.Common.UserCredentials.UserCredentialDTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Data.Repositories.UserCredentialRepositories
{
    public class UserCredentialRepository : IUserCredentialRepository
    {

        public UserCredentialRepository() { }



        public int AddUserCredential(Activity activity, UserCredentialDTO userCredential)
        {
            throw new NotImplementedException();
        }
    }
}
