using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.IInputValidationResults
{
    public interface IInputValidationResult
    {
        void AddErrorMessage(string message);
        string GetAllErrorMessages();

    }
}
