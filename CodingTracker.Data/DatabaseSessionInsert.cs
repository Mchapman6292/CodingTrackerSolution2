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

namespace CodingTracker.Data.DatabaseSessionInserts
{
    public class DatabaseSessionInsert : IDatabaseSessionInsert
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IApplicationLogger _appLogger;
        private readonly CodingSessionDTO _codingSessionDTO;
        private readonly ICodingSessionDTOManager _codingSessionDTOManager;




        public DatabaseSessionInsert(IDatabaseManager databaseManager, IApplicationLogger appLogger, ICodingSessionDTOManager codingSessionDTOManager)
        {
            _databaseManager = databaseManager;
            _appLogger = appLogger;
            _codingSessionDTOManager = codingSessionDTOManager;
        }


        public void InsertSession()
        {
            CodingSessionDTO codingSessionDTO = _codingSessionDTOManager.GetCurrentSessionDTO();

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO CodingSessions 
                    (
                        UserId, 
                        StartTime, 
                        EndTime,
                        DurationSeconds
                    ) 
                    VALUES 
                    (
                        @UserId, 
                        @StartTime, 
                        @EndTime, 
                        @DurationSeconds
                    )";

                command.Parameters.AddWithValue("@UserId", codingSessionDTO.UserId);
                command.Parameters.AddWithValue("@StartTime", codingSessionDTO.StartTime);
                command.Parameters.AddWithValue("@EndTime", codingSessionDTO.EndTime.HasValue ? (object)codingSessionDTO.EndTime.Value : DBNull.Value);
                command.Parameters.AddWithValue("@DurationSeconds", codingSessionDTO.DurationSeconds);

                command.ExecuteNonQuery();

                _appLogger.Debug($"Session inserted successfully User ID: {codingSessionDTO.UserId}, StartTime: {codingSessionDTO.StartTime}, EndTime: {codingSessionDTO.EndTime}, DurationSeconds: {codingSessionDTO.DurationSeconds}");
            }, nameof(InsertSession));
        }
    }
}
