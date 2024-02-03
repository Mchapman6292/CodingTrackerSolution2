using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace CodingTracker.Business.IInputValidators
{
    public interface IInputValidator
    {

        bool CheckDateInput(string input, out DateTime result);
        bool CheckTimeInput(string input, out DateTime result);
        public void CheckLoginInput();
        public bool CheckStartInput(string startInput);
        public bool CheckViewInput(string viewInput);
        public bool CheckSessionId(string sessionId);


        DateTime GetValidDateFromUser();
        DateTime GetValidTimeFromUser();



    }
}