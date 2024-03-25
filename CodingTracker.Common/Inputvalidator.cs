using System.Globalization;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.IDatabaseManagers;
using System.Text.RegularExpressions;
using CodingTracker.Common.IApplicationLoggers;
using System.Diagnostics;



//test

namespace CodingTracker.Common.InputValidators
{

    public class InputValidator : IInputValidator
    {
        private readonly IApplicationLogger _appLogger;


        public InputValidator(IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
        }

        public bool ValidateUsername(string username)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(ValidateUsername)).Start())
            {
                try
                {
                    _appLogger.Debug($"Starting {nameof(ValidateUsername)}. TraceID: {activity.TraceId}");

                    if (string.IsNullOrWhiteSpace(username) || username.Length > 15 || !char.IsUpper(username[0]))
                    {
                        throw new ArgumentException("Invalid username");
                    }

                    stopwatch.Stop();
                    _appLogger.Info($"Username validated successfully. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    return true;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Validation failed for username. Error: {ex.Message}. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    throw;
                }
            }
        }

        public bool ValidatePassword(string password)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(ValidatePassword)).Start())
            {
                try
                {
                    _appLogger.Debug($"Starting {nameof(ValidatePassword)}. TraceID: {activity.TraceId}");

                    if (string.IsNullOrWhiteSpace(password) || password.Length > 15)
                    {
                        throw new ArgumentException("Invalid password length");
                    }

                    if (!password.Any(char.IsUpper) || !new Regex("[^a-zA-Z0-9]").IsMatch(password))
                    {
                        throw new ArgumentException("Password must contain one capital letter and one special character");
                    }

                    stopwatch.Stop();
                    _appLogger.Info($"Password validated successfully. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    return true;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Validation failed for password. Error: {ex.Message}. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    throw;
                }
            }
        }
        public bool CheckDateInput(string input, out DateTime result)
        {
            return DateTime.TryParseExact(input, "yy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }

        public bool CheckTimeInput(string input, out DateTime result)
        {
            return DateTime.TryParseExact(input, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }


        public bool IsValidTimeFormatHHMM(string input)
        {
            // Regex pattern to match a time format of HH:mm
            string pattern = @"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$";
            return Regex.IsMatch(input, pattern);
        }

        public bool TryParseTime(string input, out TimeSpan timeSpan)
        {
            timeSpan = default;

            if (IsValidTimeFormatHHMM(input))
            {
                string[] parts = input.Split(':');
                if (int.TryParse(parts[0], out int hours) && int.TryParse(parts[1], out int minutes))
                {
                    timeSpan = new TimeSpan(hours, minutes, 0);
                    return true;
                }
            }

            return false;
        }





        public DateTime GetValidDateFromUser()
        {
            while (true)
            {
                Console.WriteLine("Enter enter date start time (yy-MM-dd):");
                string userInput = Console.ReadLine();

                if (CheckDateInput(userInput, out DateTime result))
                {
                    return result;
                }

                Console.WriteLine("Invalid date format please enter in format yy-MM-dd");
            }
        }

        public DateTime GetValidTimeFromUser() // refactor to remove writeline
        {
            while (true)
            {
                string userInput = Console.ReadLine();

                if (CheckTimeInput(userInput, out DateTime result))
                {
                    return result;
                }

                Console.WriteLine("Invalid time format. Please try again.");
            }
        }
    }
}