using CodingTracker.Common.IUserCredentialDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.UserCredentialDTOs
{
    public class UserCredentialDTO : IUserCredentialDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        public string Password { get; set; }


    }
}
