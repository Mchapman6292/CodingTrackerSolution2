using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IInputValidationResults;

// This class encapsulates the result of a valid operation.
// When a validation method (like ValidateUsername or ValidatePassword) is called, an InputValidationResult object is instantiated.
// If an error message is added to ErrorMessages list the IsValid bool is set to false.

namespace CodingTracker.Common.InputValidationResults
{
    public class InputValidationResult : IInputValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();



        public InputValidationResult()
        {
            IsValid = true;
            ErrorMessages = new List<string>();
        }

        public void AddErrorMessage(string message)
        {
            IsValid = false;
            ErrorMessages.Add(message);
        }

        public string GetAllErrorMessages()
        {
            return string.Join(Environment.NewLine, ErrorMessages);
        }
    }
}

