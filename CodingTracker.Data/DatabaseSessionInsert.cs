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


        public void InsertSession()
        {
            CodingSessionDTO codingSessionDTO = _codingSessionDTOManager.GetCurrentSessionDTO();
            CodingGoalDTO codingGoalDTO = _codingGoalDTOManager.GetCurrentCodingGoalDTO();

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO CodingSessions 
                    (
                        UserId, 
                        StartTime, 
                        EndTime,
                        DurationSeconds,
                        DurationHHMM,
                        GoalHHMM,
                        GoalReached
                        
                    ) 
                    VALUES 
                    (
                        @UserId, 
                        @StartTime, 
                        @EndTime, 
                        @DurationSeconds,
                        @DurationHHMM,
                        @GoalHHMM,
                        @GoalReached
                    )";

                command.Parameters.AddWithValue("@UserId", codingSessionDTO.UserId);
                command.Parameters.AddWithValue("@StartTime", codingSessionDTO.StartTime);
                command.Parameters.AddWithValue("@EndTime", codingSessionDTO.EndTime.HasValue ? (object)codingSessionDTO.EndTime.Value : DBNull.Value);
                command.Parameters.AddWithValue("@DurationSeconds", codingSessionDTO.DurationSeconds);
                command.Parameters.AddWithValue("@DurationHHMM", codingSessionDTO.DurationHHMM);
                command.Parameters.AddWithValue("@GoalHHMM", codingSessionDTO.GoalHHMM);
                command.Parameters.AddWithValue("@GoalReached", codingSessionDTO.GoalReached);

                command.ExecuteNonQuery();

                _appLogger.Debug($"Session inserted successfully User ID: {codingSessionDTO.UserId}, StartTime: {codingSessionDTO.StartTime}, EndTime: {codingSessionDTO.EndTime}, DurationSeconds: {codingSessionDTO.DurationSeconds}, DurationHHMM: {codingSessionDTO.DurationHHMM}.");
            }, nameof(InsertSession));
        }
    }
}
