using CodingTracker.Common;
using System.Data.SQLite;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IDatabaseSessionDeletes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using CodingTracker.Common.IApplicationLoggers;

namespace CodingTracker.Data.DatabaseSessionDeletes
{
    public class DatabaseSessionDelete : IDatabaseSessionDelete
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseManager _databaseManager;
        private readonly CodingSessionDTO _codingSessionDTO;

        public DatabaseSessionDelete(IDatabaseManager databaseManager, CodingSessionDTO codingSessionDTO, IApplicationLogger appLogger)
        {
            _databaseManager = databaseManager;
            _codingSessionDTO = codingSessionDTO;
            _appLogger = appLogger;
        }

        public void DeleteSession(CodingSessionDTO codingSessionDTO)
        {
            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"

            DELETE FROM 
                        CodingSessions 
                WHERE 
                        SessionId = @SessionId AND 
                        UserId = @UserId";

                command.Parameters.AddWithValue("@SessionId", codingSessionDTO.SessionId);
                command.Parameters.AddWithValue("@UserId", codingSessionDTO.UserId);
                command.ExecuteNonQuery();
            }, nameof(DeleteSession));
        }
    }
}
