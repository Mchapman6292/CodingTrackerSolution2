using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IUtilityServices;



namespace CodingTracker.Common.UtilityServices
{
    public class UtilityService : IUtilityService
    {
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
    }