using System.Globalization;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.IDatabaseManagers;
using System.Text.RegularExpressions;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IInputValidationResults;
using System.Diagnostics;
using System.Text;
using CodingTracker.Common.InputValidationResults;
using System.ComponentModel.DataAnnotations;



// performs the actual validation of user inputs

namespace CodingTracker.Common.InputValidators
{

    public class InputValidator : IInputValidator
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IInputValidationResult _validResult;

        public InputValidator(IApplicationLogger appLogger, IInputValidationResult validResult)
        {
            _appLogger = appLogger;
            _validResult = validResult;
        }

        public InputValidationResult ValidateUsername(string username) // Username requirements = Less than 15 chars long, begins with capital letter & not empty/no whitespaces.
        {
            var result = new InputValidationResult();
            Stopwatch stopwatch = Stopwatch.StartNew();

            using (var activity = new Activity(nameof(ValidateUsername)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ValidateUsername)}. TraceID: {activity.TraceId}");

                if (string.IsNullOrWhiteSpace(username))
                {
                    result.AddErrorMessage("Username cannot be empty or whitespace.");
                }

                if (username.Length > 15)
                {
                    result.AddErrorMessage("Username must be 15 characters or less.");
                }

                if (!string.IsNullOrWhiteSpace(username) && !char.IsUpper(username[0]))
                {
                    result.AddErrorMessage("Username must begin with a capital letter.");
                }

                stopwatch.Stop();
                _appLogger.Info($"Username validation completed. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
            }

            return result;
        }

        public InputValidationResult ValidatePassword(string password) // requirements <= 15, one upper case letter, one special char & no whitespaces/ not empty. 
        {
            var result = new InputValidationResult();
            Stopwatch stopwatch = Stopwatch.StartNew();

            using (var activity = new Activity(nameof(ValidatePassword)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ValidatePassword)}. TraceID: {activity.TraceId}");

                if (string.IsNullOrWhiteSpace(password))
                {
                    result.AddErrorMessage("Password cannot be empty or whitespace.");
                }

                if (password.Length > 15)
                {
                    result.AddErrorMessage("Password must be 15 characters or less.");
                }

                if (!password.Any(char.IsUpper))
                {
                    result.AddErrorMessage("Password must contain at least one uppercase letter.");
                }

                if (!new Regex("[^a-zA-Z0-9]").IsMatch(password))
                {
                    result.AddErrorMessage("Password must contain at least one special character.");
                }

                stopwatch.Stop();
                _appLogger.Info($"Password validation completed. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
            }

            return result;
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
            // Matches to format of HH:mm
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