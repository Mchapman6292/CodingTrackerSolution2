using Serilog;
using Serilog.Context;
using System;
using System.Diagnostics;
using CodingTracker.Common.IApplicationLoggers;

namespace CodingTracker.Logging.ApplicationLoggers
{
    public class ApplicationLogger : IApplicationLogger
    {
        private readonly Serilog.ILogger _logger;

        public ApplicationLogger()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information() // Set minimum level to Information
                .Enrich.FromLogContext() // Enrich logs from the log context
                .WriteTo.Console() // Log to console
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day) // Log to file, with daily roll-over
                .CreateLogger();
        }

        public void LogActivity(string methodName, Action<Activity> logAction, Action action)
        {
            using (var activity = new Activity(methodName).Start())
            {
                try
                {
                    logAction?.Invoke(activity);
                    action?.Invoke();
                }
                catch (Exception ex)
                {
                    Error($"Exception in {methodName}. TraceID: {activity.TraceId}", ex);
                    throw;
                }
            }
        }

        // Method overloading, allows mulitple methods with same name but different parameter lists. 
        public void Info(string message) => _logger.Information(message); // General information regarding app operations, e.g user login.
        public void Info(string message, params object[] propertyValues) => _logger.Information(message, propertyValues);
        public void Debug(string message) => _logger.Debug(message); // Detailed info used for debugging, e.g loaded X variables from database in 200ms/
        public void Debug(string message, params object[] propertyValues) => _logger.Debug(message, propertyValues);
        public void Warning(string message) => _logger.Warning(message); // Unexpected events that might lead to problems in future e.g Disk space running low.
        public void Warning(string message, params object[] propertyValues) => _logger.Warning(message, propertyValues);
        public void Error(string message, Exception ex) => _logger.Error(ex, message); // Errors/exceptions that cannot be handled/interupt execution of current operation, e.g Disk space running low.
        public void Error(string message, Exception ex, params object[] propertyValues) => _logger.Error(ex, message, propertyValues);
        public void Error(string message, params object[] propertyValues) => _logger.Error(message, propertyValues); //Cases with no exceptions for general logging.
        public void Fatal(string message) => _logger.Fatal(message); // Critical issues that cause app to stop running/operate in degraded state, should be used sparingly
        public void Fatal(string message, Exception ex) => _logger.Fatal(ex, message);
    }
}
