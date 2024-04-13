using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IErrorHandlers;
using System.Drawing;

namespace CodingTracker.Business.PanelColorControls
{
    public enum SessionColor
    {
        Grey,        // For 0 minutes
        RedGrey,     // For less than 60 minutes
        Red,         // For 1 to less than 2 hours
        Yellow,      // For 2 to less than 3 hours
        Green        // For 3 hours and more
    }
    public interface IPanelColorControl
    {
        SessionColor DetermineSessionColor(int sessionDurationMinutes);

        Color GetColorFromSessionColor(SessionColor color);
    }



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
            var activity = new Activity(nameof(DetermineSessionColor)).Start();
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _appLogger.Debug($"Determining session color for duration: {sessionDurationMinutes} minutes. TraceID: {activity.TraceId}");

                SessionColor color;
                if (sessionDurationMinutes <= 0)
                {
                    color = SessionColor.Grey;
                }
                else if (sessionDurationMinutes < 60)
                {
                    color = SessionColor.RedGrey;
                }
                else if (sessionDurationMinutes < 120)
                {
                    color = SessionColor.Red;
                }
                else if (sessionDurationMinutes < 180)
                {
                    color = SessionColor.Yellow;
                }
                else
                {
                    color = SessionColor.Green;
                }

                stopwatch.Stop();
                _appLogger.Info($"Session color determined: {color}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");

                return color;
            }
            finally
            {
                stopwatch.Stop();
                activity.Stop();
            }
        }

        public Color GetColorFromSessionColor(SessionColor color)
        {
            var activity = new Activity(nameof(GetColorFromSessionColor)).Start();
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _appLogger.Debug($"Getting color for SessionColor: {color}. TraceID: {activity.TraceId}");

                Color result;
                switch (color)
                {
                    case SessionColor.Grey:
                        result = Color.Gray;
                        break;
                    case SessionColor.RedGrey:
                        result = Color.FromArgb(255, 128, 128);
                        break;
                    case SessionColor.Red:
                        result = Color.Red;
                        break;
                    case SessionColor.Yellow:
                        result = Color.Yellow;
                        break;
                    case SessionColor.Green:
                        result = Color.Green;
                        break;
                    default:
                        result = Color.Gray;
                        break;
                }

                stopwatch.Stop();
                _appLogger.Info($"Color determined for SessionColor {color}: {result}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");

                return result;
            }
            finally
            {
                stopwatch.Stop();
                activity.Stop();
            }
        }
    }
}

