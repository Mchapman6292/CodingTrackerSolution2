using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using CodingTracker.IInputvalidator;
using Spectre.Console.Rendering;
using IValidator = CodingTracker.Business.IInputValidators.IInputValidator;



namespace CodingTracker.Business.InputValidators
{
    public class InputValidator : IValidator
    {
        private readonly IDbManager _databaseManager;
        private readonly HashSet<string> startValidCommands = new HashSet<string> { "O", "1", "2", "3" };
        private readonly HashSet<string> viewValidCommands = new HashSet<string> { "0", "1", "2", "3", };


        public InputValidator(IDbManager databaseManager)
        {
            _databaseManager = databaseManager;
        }


        public bool CheckDateInput(string input, out DateTime result)
        {
            return DateTime.TryParseExact(input, "yy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }

        public bool CheckTimeInput(string input, out DateTime result)
        {
            return DateTime.TryParseExact(input, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }

        public void CheckLoginInput()
        {
            throw new NotImplementedException();
        }


        public bool ValidateSessionId(string input)
        {
            if (int.TryParse(input, out int sessionId))
            {
                return _databaseManager.CheckSessionIdExist(sessionId);
            }
            return false;
        }
    }

    public bool CheckStartInput(string startInput)
    {


        if (!startValidCommands.Contains(startInput))
        {
            Console.WriteLine("Please enter valid option");
        }
        return true;
    }

    public bool CheckViewInput(string viewInput)
    {
        if (!viewValidCommands.Contains(viewInput))
        {
            Console.WriteLine("Please enter valid option");
        }
        return true;
    }

