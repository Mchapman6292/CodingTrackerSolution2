using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IErrorHandlers;

///If there is a higher-level catch block in the call stack that can handle the rethrown exception, the application will continue running under the control of that 
///Delegate = is a type that represents references to methods with a particular parameter list and return type. 
namespace CodingTracker.Common.ErrorHandlers
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly IApplicationLogger _appLogger;


        public ErrorHandler(IApplicationLogger applogger)
        {
            _appLogger = applogger;
        }

        // Needs to catch the most specific exceptions first
        public void CatchErrorsAndLogWithStopwatch(Action action, string methodName, bool isDatabaseOperation = false) // Action is a delegate that represents a method that returns void
        {
            using var activity = new Activity(methodName).Start();
            _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}");

            var stopwatch = Stopwatch.StartNew();

            try
            {
                action();
            }

            catch (InvalidOperationException ex)
            {
                stopwatch.Stop();
                _appLogger.Error($"Invalid operation in {methodName}: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                throw;
            }

            catch (SQLiteException ex) when (isDatabaseOperation)
            {
                stopwatch.Stop();
                _appLogger.Error($"SQLite error in {methodName}: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
            }

            catch (Exception ex)
            {
                stopwatch.Stop();
                _appLogger.Error($"Error in {methodName}: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                if (!isDatabaseOperation)
                {
                    throw;
                }
            }
            finally
            {
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                }
            }
        }

        public T CatchErrorsAndLogWithStopwatch<T>(Func<T> function, string methodName, bool isDatabaseOperation = false)
        {
            using var activity = new Activity(methodName).Start();
            _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}");

            var stopwatch = Stopwatch.StartNew();
            T result = default; // Initialize the result variable to hold the function's return value

            try
            {
                result = function(); // Execute the function and store the return value
            }
            catch (InvalidOperationException ex)
            {
                stopwatch.Stop();
                _appLogger.Error($"Invalid operation in {methodName}: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                throw;
            }
            catch (SQLiteException ex) when (isDatabaseOperation)
            {
                stopwatch.Stop();
                _appLogger.Error($"SQLite error in {methodName}: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _appLogger.Error($"Error in {methodName}: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                if (!isDatabaseOperation)
                {
                    throw; // Rethrow the exception for non-database operations
                }
            }
            finally
            {
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                }
                _appLogger.Debug($"Finished {methodName}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
            }

            return result; // Return the result of the function execution
        }

        public void HandleErrors(Action action, string methodName)
        {
            try
            {
                action();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid operation in {methodName}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in {methodName}: {ex.Message}");
                throw;
            }
        }


        public void HandleErrorsWithSql(Action action, string methodName)
        {
            try
            {
                action();
            }
            catch (SQLiteException ex)
            {

                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in {methodName} (DB Operation): {ex.Message}");
                throw;
            }
        }
    }
}
   
