using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IPanelColorControls;
using CodingTracker.Common.IErrorHandlers;
using System.Drawing;

namespace CodingTracker.Business.PanelColorControls
{ 
    public class PanelColorControl : IPanelColorControl
{
        private readonly IApplicationLogger _appLogger;
        private readonly IErrorHandler _errorHandler;


        public PanelColorControl(IApplicationLogger appLogger, IErrorHandler errorHandler) 
        {
            _appLogger = appLogger;
            _errorHandler = errorHandler;
        }

        public SessionColor DetermineSessionColor(int sessionDurationMinutes)
        {
            if (sessionDurationMinutes <= 0)
            {
                return SessionColor.Grey; // 0 minutes
            }
            else if (sessionDurationMinutes < 60)
            {
                return SessionColor.RedGrey; // Less than 60 minutes
            }
            else if (sessionDurationMinutes < 120) // 1 to less than 2 hours
            {
                return SessionColor.Red;
            }
            else if (sessionDurationMinutes < 180) // 2 to less than 3 hours
            {
                return SessionColor.Yellow;
            }
            else // 3 hours and more
            {
                return SessionColor.Green;
            }
        }

        public Color GetColorFromSessionColor(SessionColor color)
        {
            switch (color)
            {
                case SessionColor.Grey:
                    return Color.Gray;
                case SessionColor.RedGrey:
                    return Color.FromArgb(255, 128, 128); // RGB for red/grey. 
                case SessionColor.Red:
                    return Color.Red;
                case SessionColor.Yellow:
                    return Color.Yellow;
                case SessionColor.Green:
                    return Color.Green;
                default:
                    return Color.Gray; // Default color
            }
        }
    }
}

