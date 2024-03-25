using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.IInputValidators
{
    public interface IInputValidator 
    {

        bool CheckDateInput(string input, out DateTime result);
        bool CheckTimeInput(string input, out DateTime result);
        bool IsValidTimeFormatHHMM(string input);
        bool TryParseTime(string input, out TimeSpan timeSpan);

        bool ValidatePassword(string password);
        bool ValidateUsername(string username);



        DateTime GetValidDateFromUser();
        DateTime GetValidTimeFromUser();



    }
}