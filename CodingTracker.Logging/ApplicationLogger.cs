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
                .MinimumLevel.Debug() 
                .Enrich.FromLogContext() 
                .WriteTo.Console() 
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day) 
                .CreateLogger();
        }

        public void LogActivity(string methodName, Action<Activity> logAction, Action<Activity> action)
        {
            using (var activity = new Activity(methodName).Start())
            {
                try
                {
                    logAction?.Invoke(activity);
                    action?.Invoke(activity);
                }
                catch (Exception ex)
                {
                    Error($"Exception in {methodName}. TraceID: {activity.TraceId}", ex);
                    throw;
                }
            }
        }

        public async Task LogActivityAsync(string methodName, Func<Activity, Task> logAction, Func<Activity, Task> action)
        {
            Activity activity = Activity.Current ?? new Activity(methodName).Start();

            try
            {
                if (logAction != null)
                    await logAction(activity);
                if (action != null)
                    await action(activity);
            }
            catch (Exception ex)
            {
                Error($"Exception in {methodName}. TraceID: {activity.TraceId}", ex);
                throw;
            }
            finally
            {
                if (Activity.Current == null)
                    activity.Stop();
            }
        }


        private void LogDatabaseError(Exception ex, string operationName, Stopwatch stopwatch)
        {
            stopwatch.Stop();
            Error($"Error executing {operationName}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {Activity.Current?.TraceId}");
        }

        public void LogUpdates(string methodName, params (string Name, object Value)[] updates)
        {
            using (var activity = new Activity(nameof(LogUpdates)).Start())
            {
                var updateEntries = updates
                    .Where(update => update.Value != null)
                    .Select(update => $"{update.Name}={update.Value}")
                    .ToList();

                if (updateEntries.Count > 0)
                {
                    string message = $"Updated {methodName}: {string.Join(", ", updateEntries)}. TraceID: {activity.TraceId}";
                    Info(message);
                }
            }
        }


        public Task LogUpdatesAsync(string methodName, params (string Name, object Value)[] updates)
        {
            using (var activity = new Activity(nameof(LogUpdatesAsync)).Start())
            {
                var updateEntries = updates
                    .Where(update => update.Value != null)
                    .Select(update => $"{update.Name}={update.Value}")
                    .ToList();
                if (updateEntries.Count > 0)
                {
                    string message = $"Updated {methodName}: {string.Join(", ", updateEntries)}. TraceID: {activity.TraceId}";
                    Info(message); 
                }
            }
            return Task.CompletedTask;
        }


        // Method overloading, allows multiple methods with same name but different parameter lists. 
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
