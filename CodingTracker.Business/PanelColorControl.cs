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
using CodingTracker.Common.CodingSessionDTOs;

namespace CodingTracker.Business.PanelColorControls
{
    public enum SessionColor
    {
        Blue,        // For 0 minutes
        RedGrey,     // For less than 60 minutes
        Red,         // For 1 to less than 2 hours
        Yellow,      // For 2 to less than 3 hours
        Green,       // For 3 hours and more
        Black        // For errors/null   
    }
    public interface IPanelColorControl
    {

        List<SessionColor> AssignColorsToSessionsInLast28Days();
        Color ConvertSessionColorToColor(SessionColor color);

        SessionColor DetermineSessionColor(double? sessionDurationSeconds);
    }



    public class PanelColorControl : IPanelColorControl
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabaseSessionRead _databaseSessionRead;
        private readonly List<(DateTime Day, double TotalDurationMinutes)> _dailyDurations;
        private readonly List<SessionColor> _sessionColors;



        public PanelColorControl(IApplicationLogger appLogger, IErrorHandler errorHandler, IDatabaseSessionRead databaseSessionRead)
        {
            _appLogger = appLogger;
            _errorHandler = errorHandler;
            _databaseSessionRead = databaseSessionRead;
            _dailyDurations = _databaseSessionRead.ReadTotalSessionDurationByDay();
        }

        public List<SessionColor> AssignColorsToSessionsInLast28Days()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(AssignColorsToSessionsInLast28Days)))
            {
                _appLogger.Info($"Starting {nameof(AssignColorsToSessionsInLast28Days)}, TraceID: {activity.TraceId}.");

                List<(DateTime Date, double TotalDurationSeconds)> dailyDurations = _databaseSessionRead.ReadTotalSessionDurationByDay();
                List<SessionColor> sessionColors = new List<SessionColor>();


                foreach (var dayDuration in dailyDurations)
                {
                    SessionColor color = DetermineSessionColor(dayDuration.TotalDurationSeconds);
                    sessionColors.Add(color);

                    _appLogger.Debug($"Assigned color for day: {dayDuration.Date.ToShortDateString()}, Sum of DurationSeconds for date{dayDuration.Date} = {dayDuration.TotalDurationSeconds} , Color: {color}.");
                }

                stopwatch.Stop();
                _appLogger.Info($"Completed {nameof(AssignColorsToSessionsInLast28Days)}. Total Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}.");
                return sessionColors;
            }
        }






        public SessionColor DetermineSessionColor(double? sessionDurationSeconds)
        {
            using (new Activity(nameof(DetermineSessionColor))) { }
            if (!sessionDurationSeconds.HasValue || sessionDurationSeconds <= 0)
            {
                return SessionColor.Blue;
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


        public Color ConvertSessionColorToColor(SessionColor color)
        {
            var activity = new Activity(nameof(ConvertSessionColorToColor)).Start();
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _appLogger.Debug($"Getting color for SessionColor: {color}. TraceID: {activity.TraceId}");

                Color result;
                switch (color)
                {
                    case SessionColor.Blue:
                        result = Color.Blue;
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
                    case SessionColor.Black: // For errors/null
                        result = Color.Black;
                        break;
                    default:
                        result = Color.Blue; // Default case as fallback
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

