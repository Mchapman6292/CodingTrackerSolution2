using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.IApplicationLoggers;
using System.Diagnostics;
using CodingTracker.Common.BusinessInterfaces.ICodingSessionTimers;

namespace CodingTracker.Business.CodingSessionTimers
{
    public class CodingSessionTimer : ICodingSessionTimer
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IErrorHandler _errorHandler;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        public bool IsStopWatchEnabled = false;


        public CodingSessionTimer(IApplicationLogger appLogger, IErrorHandler errorHandler)
        {
            _appLogger = appLogger;
            _errorHandler = errorHandler;
        }

        public void StartCodingSessionTimer()
        {
            _errorHandler.CatchErrorsAndLogWithStopwatch(() =>
            {

                if (!IsStopWatchEnabled)
                {
                    _stopwatch.Start();
                };
                _appLogger.Info($"Coding session timer started.");
            }, nameof(StartCodingSessionTimer));
        }

        public void EndCodingSessionTimer()
        {
            _errorHandler.CatchErrorsAndLogWithStopwatch(() =>
            {

                if (IsStopWatchEnabled)
                {
                    _stopwatch.Stop();
                };
                _appLogger.Info($"Coding session timer stopped.");
            }, nameof(EndCodingSessionTimer));
        }
    }
}
