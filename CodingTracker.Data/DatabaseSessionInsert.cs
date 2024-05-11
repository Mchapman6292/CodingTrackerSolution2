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

            _appLogger.Debug($"Coding session DTO for {nameof(InsertSession)}: " +
                     $"userId: {codingSessionDTO.userId}, " +
                     $"startDate: {codingSessionDTO.startDate}, " +
                     $"startTime: {codingSessionDTO.startTime}, " +
                     $"endDate: {codingSessionDTO.endDate}, " +
                     $"endTime: {codingSessionDTO.endTime}, " +
                     $"durationSeconds: {codingSessionDTO.durationSeconds}, " +
                     $"durationHHMM: {codingSessionDTO.durationHHMM}, " +
                     $"goalHHMM: {codingSessionDTO.goalHHMM}, " +
                     $"goalReached: {codingSessionDTO.goalReached}");

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO CodingSessions 
                    (
                        userId, 
                        startDate,
                        startTime, 
                        endDate,
                        endTime,
                        durationSeconds,
                        durationHHMM,
                        goalHHMM,
                        goalReached
                        
                    ) 
                    VALUES 
                    (
                        @userId, 
                        @startDate,
                        @startTime,
                        @endDate,
                        @endTime, 
                        @durationSeconds,
                        @durationHHMM,
                        @goalHHMM,
                        @goalReached
                    )";

                // sql lite database does not accept null values, they require a specific representation of a null value (DBNull.Value)
                // 

                command.Parameters.AddWithValue("@userId", codingSessionDTO.userId);
                command.Parameters.AddWithValue("@startDate", codingSessionDTO.startDate.HasValue ? (object)codingSessionDTO.startDate.Value : DBNull.Value);
                command.Parameters.AddWithValue("@startTime", codingSessionDTO.startTime.HasValue ? (object)codingSessionDTO.startTime.Value : DBNull.Value);
                command.Parameters.AddWithValue("@endDate", codingSessionDTO.endDate.HasValue ? (object)codingSessionDTO.endDate.Value : DBNull.Value);
                command.Parameters.AddWithValue("@endTime", codingSessionDTO.endTime.HasValue ? (object)codingSessionDTO.endTime.Value : DBNull.Value);
                command.Parameters.AddWithValue("@durationSeconds", codingSessionDTO.durationSeconds.HasValue ? (object)codingSessionDTO.durationSeconds.Value : DBNull.Value);
                command.Parameters.AddWithValue("@durationHHMM", codingSessionDTO.durationHHMM ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@goalHHMM", codingSessionDTO.goalHHMM ?? (object)DBNull.Value);


                command.ExecuteNonQuery();

                _appLogger.Debug($"Session inserted successfully Session ID: {codingSessionDTO.userId}, startTime: {codingSessionDTO.startTime}, endTime: {codingSessionDTO.endTime}, durationSeconds: {codingSessionDTO.durationSeconds}, durationHHMM: {codingSessionDTO.durationHHMM}.");
            }, nameof(InsertSession));
        }
    }
}
