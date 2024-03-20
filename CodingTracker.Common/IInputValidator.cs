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
        public void CheckLoginInput();
        public bool CheckStartInput(string startInput);
        public bool CheckViewInput(string viewInput);



        DateTime GetValidDateFromUser();
        DateTime GetValidTimeFromUser();



    }
}