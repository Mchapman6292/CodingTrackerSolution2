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
            SessionColor resultColor = SessionColor.Red; // Default color

            _errorHandler.CatchErrorsAndLogWithStopwatch(() =>
            {
                // Your session color determination logic
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

            }, nameof(DetermineSessionColor));

            return resultColor;
        }
    }
}
