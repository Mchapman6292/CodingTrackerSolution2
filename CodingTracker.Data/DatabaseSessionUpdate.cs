using System;
using CodingTracker.Common.IDatabaseSessionUpdates;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IDatabaseManagers;
using System.Diagnostics;

namespace CodingTracker.Data.DatabaseSessionUpdates
{
    public class DatabaseSessionUpdate : IDatabaseSessionUpdate
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IApplicationLogger _appLogger;
        private readonly CodingSessionDTO _codingSessionDTO;


        public DatabaseSessionUpdate(IDatabaseManager databaseManager, IApplicationLogger appLogger, CodingSessionDTO codingSessionDTO)
        {
            _databaseManager = databaseManager;
            _appLogger = appLogger;
            _codingSessionDTO = codingSessionDTO;
        }



        public void UpdateSession()
        {
            var methodName = nameof(UpdateSession);
            using (var activity = new Activity(methodName).Start())
            {
                _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}, SessionId: {_codingSessionDTO.SessionId}, UserId: {_codingSessionDTO.UserId}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using var command = connection.CreateCommand();
                        command.ExecuteNonQuery();
                    });

                    stopwatch.Stop();
                    _appLogger.Info($"{methodName} executed successfully. SessionId: {_codingSessionDTO.SessionId}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to execute {methodName}. SessionId: {_codingSessionDTO.SessionId}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
            }
        }


        public void UpdateProgress()
        {
            var methodName = nameof(UpdateProgress);
            using (var activity = new Activity(methodName).Start())
            {
                _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}, UserId: {_codingSessionDTO.UserId}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using var command = connection.CreateCommand();
                        command.ExecuteNonQuery();
                    });

                    stopwatch.Stop();
                    _appLogger.Info($"{methodName} executed successfully. UserId: {_codingSessionDTO.UserId}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to execute {methodName}. UserId: {_codingSessionDTO.UserId}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
            }
        }
    }
}
