using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.IUtilityServices
{
    public interface IUtilityService
    {
        bool IsValidString(string input);
        int TryParseInt(string input);
        bool TryParseDate(string input, out DateTime result);

        string HashPassword(string password);


    }
}
