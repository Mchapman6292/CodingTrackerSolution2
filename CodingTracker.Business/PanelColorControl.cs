using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.IDatabaseSessionReads;
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
        SessionColor DetermineSessionColor(int sessionDurationSeconds);
        List<SessionColor> DetermineSessionColors();

        Color GetColorFromSessionColor(SessionColor color);
    }



    public class PanelColorControl : IPanelColorControl
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabaseSessionRead _databaseSessionRead;
        private readonly List<(DateTime Day, int TotalDurationMinutes)> _dailyDurations;



        public PanelColorControl(IApplicationLogger appLogger, IErrorHandler errorHandler, IDatabaseSessionRead databaseSessionRead)
        {
            _appLogger = appLogger;
            _errorHandler = errorHandler;
            _databaseSessionRead = databaseSessionRead;
            _dailyDurations = _databaseSessionRead.ReadTotalSessionDurationByDay();
        }

        public List<SessionColor> DetermineSessionColors()
        {
            var colors = new List<SessionColor>();
            foreach (var duration in _dailyDurations)
            {
                colors.Add(DetermineSessionColor(duration.TotalDurationMinutes));
            }
            return colors;
        }

        public SessionColor DetermineSessionColor(int sessionDurationSeconds)
        {
            if (sessionDurationSeconds <= 0)
            {
                return SessionColor.Grey;
            }
            else if (sessionDurationSeconds < 3600) // Less than 60 minutes
            {
                return SessionColor.RedGrey;
            }
            else if (sessionDurationSeconds < 7200) // 1 to less than 2 hours
            {
                return SessionColor.Red;
            }
            else if (sessionDurationSeconds < 10800) // 2 to less than 3 hours
            {
                return SessionColor.Yellow;
            }
            else
            {
                return SessionColor.Green; // 3 hours and more
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

