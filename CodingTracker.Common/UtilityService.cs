using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IUtilityServices;
using CodingTracker.Common.IApplicationLoggers;
using System.Security.Cryptography;



namespace CodingTracker.Common.UtilityServices
{
    public class UtilityService : IUtilityService
    {
        private readonly IApplicationLogger _appLogger;

        public UtilityService(IApplicationLogger appLogger)
        { 
            _appLogger = appLogger; 
        }
        public bool IsValidString(string input)
        {
            return !string.IsNullOrEmpty(input);
        }

        public int TryParseInt(string input)
        {
            if (!int.TryParse(input, out int result))
            {
                throw new FormatException($"Unable to parse '{input}' as an integer.");
            }
            return result;
        }

        public bool TryParseDate(string input, out DateTime result)
        {
            return DateTime.TryParseExact(input, "yy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }

        public double CalculatePercentage(double part, double total)
        {
            if (total == 0) return 0;
            return (part / total) * 100;
        }


        public string HashPassword( Activity activity, string password)
        {
            _appLogger.Debug($"Starting {nameof(HashPassword)}, TraceId: {activity.TraceId}.");
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }

                    stopwatch.Stop();
                    _appLogger.Info($"{nameof(HashPassword)} completed successfully. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}.");
                    return builder.ToString();
                }
            }
            catch (ArgumentNullException ex)
            {
                stopwatch.Stop();
                _appLogger.Error($"Password cannot be null. Error: {ex.Message}. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                throw;
            }
            catch (EncoderFallbackException ex)
            {
                stopwatch.Stop();
                _appLogger.Error($"Encoding error while hashing the password. Error: {ex.Message}. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _appLogger.Error($"An unexpected error occurred while hashing the password. Error: {ex.Message}. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                throw;
            }
        }
    }
}