
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Can use DI to pass instance as class can be defined as Data Transfer Object
// Data transfer objects (DTOs) like UserCredential should be simple and not have dependencies injected into them. Their role is often to carry data with little or no behavior (methods). If you need to perform validation or other logic on the data in UserCredential, it's usually done in a service class that handles business logic, not directly in the DTO.
namespace CodingTracker.Common.UserCredentialDTOs
{
    public class UserCredentialDTO
    {
        public int UserId { get; internal set; }
        public string Username { get; internal set; }
        public string Password { get; internal set; }



    }
}