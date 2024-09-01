using System.Diagnostics;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.IApplicationLoggers;
using System.Linq.Expressions;
using CodingTracker.Common.ICodingSessions;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.ICodingSessionTimers;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.CodingSessionDTOManagers;
using CodingTracker.Common.ICredentialManagers;
using CodingTracker.Business.SessionCalculators;
using CodingTracker.Common.IdGenerators;


// method to record start & end time
// logic to hold recorded times & view them
// user should be able to input start & end times manually j     

namespace CodingTracker.Business.CodingSessions
{
    public class SessionLogic : ISessionLogic
    {
        private readonly IInputValidator _inputValidator;
        private readonly IApplicationLogger _appLogger;
        private readonly IErrorHandler _errorHandler;
        private readonly ICodingSessionTimer _sessionTimer;

        private readonly ICredentialManager _credentialManager;
        private readonly ISessionCalculator _sessionCalculator;
        private readonly IIdGenerators _idGenerators;
        private readonly int _userId;
        private readonly int _sessionId;
        public bool IsStopWatchEnabled = false;
        private bool isCodingSessionActive = false;


        public SessionLogic(IInputValidator validator, IApplicationLogger appLogger, IErrorHandler errorHandler, ICodingSessionTimer sessionTimer,  ICredentialManager credentialManager, ISessionCalculator sessionCalculator, IIdGenerators idGenerators)
        {
            _inputValidator = validator;
            _appLogger = appLogger;
            _errorHandler = errorHandler;
            _sessionTimer = sessionTimer;
            _credentialManager = credentialManager;
            _sessionCalculator = sessionCalculator;
            _idGenerators = idGenerators;
            
        }
        

        public void StartSession()
        {
            throw new NotImplementedException();
        }

        public void EndSession()
        {
            throw new NotImplementedException();
        }



        public bool CheckIfCodingSessionActive()
        {
            using (var activity = new Activity(nameof(CheckIfCodingSessionActive)).Start())
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug($"Checking if session is active. TraceID: {activity.TraceId}");

                bool isActive = isCodingSessionActive;

                stopwatch.Stop();
                _appLogger.Info($"Coding session active status: {isActive}, Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");

                return isActive;
            }
        }



        public List<DateTime> GetDatesPrevious28days() // Potential mismatch with sql lite db dates?
        {
            using (var activity = new Activity(nameof(GetDatesPrevious28days)).Start())
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug($"Getting dates for the previous 28 days. TraceID: {activity.TraceId}");

                List<DateTime> dates = new List<DateTime>();
                DateTime today = DateTime.Today;

                for (int i = 1; i <= 29; i++)
                {
                    dates.Add(today.AddDays(-i));
                }

                stopwatch.Stop();
                _appLogger.Info($"Retrieved dates for the previous 28 days. Count: {dates.Count}, Execution Time: {stopwatch.ElapsedMilliseconds}ms, Trace ID: {activity.TraceId}");

                return dates;
            }
        }
    }
}