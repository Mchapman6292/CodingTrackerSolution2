using CodingTracker.Common;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IDatabaseSessionInserts;
using System.Data.SQLite;
using CodingTracker.Data.DatabaseManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using CodingTracker.Common.CodingSessionDTOManagers;
using CodingTracker.Common.CodingGoalDTOManagers;
using CodingTracker.Common.CodingGoalDTOs;

namespace CodingTracker.Data.DatabaseSessionInserts
{
    public class DatabaseSessionInsert : IDatabaseSessionInsert
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IApplicationLogger _appLogger;
        private readonly CodingSessionDTO _codingSessionDTO;
        private readonly ICodingSessionDTOManager _codingSessionDTOManager;
        private readonly ICodingGoalDTOManager _codingGoalDTOManager;




        public DatabaseSessionInsert(IDatabaseManager databaseManager, IApplicationLogger appLogger, ICodingSessionDTOManager codingSessionDTOManager, ICodingGoalDTOManager codingGoalDTOManager)
        {
            _databaseManager = databaseManager;
            _appLogger = appLogger;
            _codingSessionDTOManager = codingSessionDTOManager;
            _codingGoalDTOManager = codingGoalDTOManager;
        }


    
    }
}
