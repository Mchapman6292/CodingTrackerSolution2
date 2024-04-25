
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Data transfer objects (DTOs) like UserCredential should be simple and not have dependencies injected into them. Their role is often to carry data with little or no behavior (methods). If you need to perform validation or other logic on the data in UserCredential, it's usually done in a service class that handles business logic, not directly in the DTO.

namespace CodingTracker.Common.UserCredentialDTOs
{
    public class UserCredentialDTO 
    {
        public int? UserId { get; set; } = 0;
        public required string Username { get; set; }

        public required string PasswordHash { get; set; }

        public DateTime LastLogin { get; set; }




      
    }
}
