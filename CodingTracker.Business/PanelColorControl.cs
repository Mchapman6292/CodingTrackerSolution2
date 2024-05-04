using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.IDatabaseSessionReads;
using CodingTracker.Common.INewDatabaseReads;
using System.Drawing;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Data.QueryBuilders;

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

        List<Color> AssignColorsToSessionsInLast28Days();
        Color ConvertSessionColorEnumToColor(SessionColor color);

        SessionColor DetermineSessionColor(double? sessionDurationSeconds);
    }



    public class PanelColorControl : IPanelColorControl
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabaseSessionRead _databaseSessionRead;
        private readonly INewDatabaseRead _newDatabaseRead;
        private readonly List<(DateTime Day, double TotalDurationMinutes)> _dailyDurations;
        private readonly List<SessionColor> _sessionColors;



        public PanelColorControl(IApplicationLogger appLogger, IErrorHandler errorHandler, IDatabaseSessionRead databaseSessionRead, INewDatabaseRead newDatabaseRead)
        {
            _appLogger = appLogger;
            _errorHandler = errorHandler;
            _databaseSessionRead = databaseSessionRead;
            _dailyDurations = _databaseSessionRead.ReadDurationSecondsLast28Days();
            _newDatabaseRead = newDatabaseRead;
        }

        public List<Color> AssignColorsToSessionsInLast28Days()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(AssignColorsToSessionsInLast28Days)).Start())
            {
                _appLogger.Info($"Starting {nameof(AssignColorsToSessionsInLast28Days)}, TraceID: {activity.TraceId}.");

                DateTime startDate = DateTime.Now.AddDays(-28);
                DateTime endDate = DateTime.Now;
                List<string> columnsToSelect = new List<string> { "SUM(DurationSeconds) as TotalDuration", "StartTime" };
                string groupBy = "StartDate";
                string orderBy = "StartTime";

                List<CodingSessionDTO> aggregatedSessions = _newDatabaseRead.ReadFromCodingSessionsTable(
                    columnsToSelect,
                    startDate: startDate,
                    endDate: endDate,
                    groupBy: groupBy,
                    orderBy: orderBy,
                    ascending: true
                );

                _appLogger.Debug($"Sessions returned by ReadFromCodingSessionsTable for {nameof(AssignColorsToSessionsInLast28Days)}: {aggregatedSessions}.");

                List<Color> sessionColors = new List<Color>();
                foreach (var session in aggregatedSessions)
                {
                    double totalDurationSeconds = session.DurationSeconds ?? 0; 
                    DateTime? sessionDate = session.StartDate;  

                    SessionColor colorEnum = DetermineSessionColor(totalDurationSeconds);
                    Color color = ConvertSessionColorEnumToColor(colorEnum);
                    sessionColors.Add(color);

                    _appLogger.Debug($"Assigned color for day: {sessionDate?.ToString("yyyy-MM-dd")}, DurationSeconds: {totalDurationSeconds}, Color: {color}.");
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


        public Color ConvertSessionColorEnumToColor(SessionColor color)
        {
            var activity = new Activity(nameof(ConvertSessionColorEnumToColor)).Start();
            var stopwatch = Stopwatch.StartNew();

            _appLogger.Debug($"Starting {nameof(ConvertSessionColorEnumToColor)} ceID: {activity.TraceId}");
            try
            {

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

