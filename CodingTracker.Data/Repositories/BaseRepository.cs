using CodingTracker.Common.IApplicationLoggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Data.Repositories
{
    public class BaseRepository
    {
        private readonly IApplicationLogger _appLogger;
        



        public BaseRepository(IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
        }
    }
}
