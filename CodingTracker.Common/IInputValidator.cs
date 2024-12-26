﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.BusinessInterfaces;

namespace CodingTracker.Common.IInputValidators
{
    public interface IInputValidator 
    {

        bool CheckDateInput(string input, out DateTime result);
        bool CheckTimeInput(string input, out DateTime result);
        bool IsValidTimeFormatHHMM(string input);
        bool TryParseTime(string input, out TimeSpan timeSpan);

        InputValidationResult ValidateUsername(string username);

        InputValidationResult ValidatePassword(string password);

        int? ParseHHMMStringInputToInt(string input);

        DateTime GetValidDateFromUser();
        DateTime GetValidTimeFromUser();



    }
}