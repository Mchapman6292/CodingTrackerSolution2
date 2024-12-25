using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Data.Repositories.CodingSessionRepositories;
using System.Drawing;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Data.QueryBuilders;
using CodingTracker.Common.DataInterfaces.ICodingSessionRepositories;

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
        Task<List<Color>> AssignColorsToSessionsInLast28Days();
        Color ConvertSessionColorEnumToColor(SessionColor color);

        SessionColor DetermineSessionColor(double? sessionDurationSeconds);

        List<DateTime> GetDatesPrevious28days();



    }



    public class PanelColorControl : IPanelColorControl
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IErrorHandler _errorHandler;
        private readonly List<(DateTime Day, double TotalDurationMinutes)> _dailyDurations;
        private readonly List<SessionColor> _sessionColors;
        private readonly ICodingSessionRepository _codingSessionRepository;



        public PanelColorControl(IApplicationLogger appLogger, IErrorHandler errorHandler, ICodingSessionRepository codingSessionRepository)
        {
            _appLogger = appLogger;
            _errorHandler = errorHandler;
            _codingSessionRepository = codingSessionRepository;
        }

        public async Task<List<Color>> AssignColorsToSessionsInLast28Days()
        {
            using (var activity = new Activity(nameof(AssignColorsToSessionsInLast28Days)).Start())
            {
                _appLogger.Info($"Starting {nameof(AssignColorsToSessionsInLast28Days)}, TraceID: {activity.TraceId}.");


                var recentSessions = await _codingSessionRepository.GetRecentSessionsAsync(28);

  
                _appLogger.Debug($"Sessions returned by ReadFromCodingSessionsTable for {nameof(AssignColorsToSessionsInLast28Days)}: {recentSessions}.");

                List<Color> sessionColors = new List<Color>();
                foreach (var session in recentSessions)
                {
                    double totalDurationSeconds = session.DurationSeconds ?? 0;

                    DateOnly? sessionDate = session.StartDate.HasValue
                            ? DateOnly.FromDateTime(session.StartDate.Value)
                            : null;

                    SessionColor colorEnum = DetermineSessionColor(totalDurationSeconds);
                    Color color = ConvertSessionColorEnumToColor(colorEnum);
                    sessionColors.Add(color);

                    _appLogger.Debug($"Assigned color for day: {sessionDate?.ToString("yyyy-MM-dd")}, DurationSeconds: {totalDurationSeconds}, Color: {color}.");
                }
                _appLogger.Info($"Completed {nameof(AssignColorsToSessionsInLast28Days)}.TraceID: {activity.TraceId}.");
                return sessionColors;
            }
        }







        public SessionColor DetermineSessionColor(double? sessionDurationSeconds)
        {
            using (new Activity(nameof(DetermineSessionColor))) { }
            if (!sessionDurationSeconds.HasValue || sessionDurationSeconds <= 0)
            {
                return SessionColor.Green;
            }
            else if (sessionDurationSeconds < 3600) 
            {
                return SessionColor.RedGrey;
            }
            else if (sessionDurationSeconds < 7200) 
            {
                return SessionColor.Red;
            }
            else if (sessionDurationSeconds < 10800) 
            {
                return SessionColor.Yellow;
            }
            else
            {
                return SessionColor.Green; 
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

