using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IPanelColorControls;
using CodingTracker.Common.IErrorHandlers;

namespace CodingTracker.Business.PanelColorControls
{ 
    public class PanelColorControl : IPanelColorControl
{
        private readonly IApplicationLogger _appLogger;
        private readonly IErrorHandler _errorHandler;
        public enum SessionColor
        {
            Red,
            Yellow,
            Green
        }


        public PanelColorControl(IApplicationLogger appLogger, IErrorHandler errorHandler) 
        {
            _appLogger = appLogger;
            _errorHandler = errorHandler;
        }

        public SessionColor DetermineSessionColor(TimeSpan sessionDuration)
        {
            _errorHandler.CatchErrorsAndLogWithStopwatch()) =>
                { 
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(DetermineSessionColor)).Start())
            {
                _appLogger.Debug($"Starting {nameof(DetermineSessionColor)} with session duration: {sessionDuration}. TraceID: {activity.TraceId}");

                SessionColor resultColor;

                    try
                    {
                        if (sessionDuration < TimeSpan.FromHours(1))
                        {
                            resultColor = SessionColor.Red;
                        }
                        else if (sessionDuration < TimeSpan.FromHours(2))
                        {
                            resultColor = SessionColor.Yellow;
                        }
                        else
                        {
                            resultColor = SessionColor.Green;
                        }

                        stopwatch.Stop();
                        _appLogger.Info($"Determined color: {resultColor}. Duration: {sessionDuration}. Process Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                        return resultColor;
                    }
                    catch (Exception ex)
                    {
                        stopwatch.Stop();
                        _appLogger.Error($"Error in determining session color: {ex.Message}. Duration: {sessionDuration}. Process Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                        throw;
                    }
                }
            }
        }
    }
}
