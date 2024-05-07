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
                     $"UserId: {codingSessionDTO.UserId}, " +
                     $"StartDate: {codingSessionDTO.StartDate}, " +
                     $"StartTime: {codingSessionDTO.StartTime}, " +
                     $"EndDate: {codingSessionDTO.EndDate}, " +
                     $"EndTime: {codingSessionDTO.EndTime}, " +
                     $"DurationSeconds: {codingSessionDTO.DurationSeconds}, " +
                     $"DurationHHMM: {codingSessionDTO.DurationHHMM}, " +
                     $"GoalHHMM: {codingSessionDTO.GoalHHMM}, " +
                     $"GoalReached: {codingSessionDTO.GoalReached}");

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO CodingSessions 
                    (
                        UserId, 
                        StartDate,
                        StartTime, 
                        EndDate,
                        EndTime,
                        DurationSeconds,
                        DurationHHMM,
                        GoalHHMM,
                        GoalReached
                        
                    ) 
                    VALUES 
                    (
                        @UserId, 
                        @StartDate,
                        @StartTime,
                        @EndDate,
                        @EndTime, 
                        @DurationSeconds,
                        @DurationHHMM,
                        @GoalHHMM,
                        @GoalReached
                    )";

                // sql lite database does not accept null values, they require a specific representation of a null value (DBNull.Value)
                // 

                command.Parameters.AddWithValue("@UserId", codingSessionDTO.UserId);
                command.Parameters.AddWithValue("@StartDate", codingSessionDTO.StartDate.HasValue ? (object)codingSessionDTO.StartDate.Value : DBNull.Value);
                command.Parameters.AddWithValue("@StartTime", codingSessionDTO.StartTime.HasValue ? (object)codingSessionDTO.StartTime.Value : DBNull.Value);
                command.Parameters.AddWithValue("@EndDate", codingSessionDTO.EndDate.HasValue ? (object)codingSessionDTO.EndDate.Value : DBNull.Value);
                command.Parameters.AddWithValue("@EndTime", codingSessionDTO.EndTime.HasValue ? (object)codingSessionDTO.EndTime.Value : DBNull.Value);
                command.Parameters.AddWithValue("@DurationSeconds", codingSessionDTO.DurationSeconds.HasValue ? (object)codingSessionDTO.DurationSeconds.Value : DBNull.Value);
                command.Parameters.AddWithValue("@DurationHHMM", codingSessionDTO.DurationHHMM ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@GoalHHMM", codingSessionDTO.GoalHHMM ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@GoalReached", codingSessionDTO.GoalReached.HasValue ? (object)codingSessionDTO.GoalReached.Value : DBNull.Value);


                command.ExecuteNonQuery();

                _appLogger.Debug($"Session inserted successfully Session ID: {codingSessionDTO.UserId}, StartTime: {codingSessionDTO.StartTime}, EndTime: {codingSessionDTO.EndTime}, DurationSeconds: {codingSessionDTO.DurationSeconds}, DurationHHMM: {codingSessionDTO.DurationHHMM}.");
            }, nameof(InsertSession));
        }
    }
}
