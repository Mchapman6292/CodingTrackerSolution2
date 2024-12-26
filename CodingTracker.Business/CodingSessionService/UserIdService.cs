using CodingTracker.Common.IApplicationLoggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Business.CodingSessionService.UserIdServices
{
    public class UserIdService
    {
        private readonly IApplicationLogger _appLogger;
        private  int _currentUserId {  get; set; }  


        public UserIdService(IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
        }

        public int GetCurrentUserId()
        {
            return _currentUserId;
        }

        public void SetCurrentUserId(int userId)
        {
            _currentUserId = userId;
        }


    }
}
