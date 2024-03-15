using System.Globalization;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.IDatabaseManagers;



//test

namespace CodingTracker.Business.InputValidators
{
    public class InputValidator : IInputValidator
    {
        private readonly HashSet<string> startValidCommands = new HashSet<string> { "O", "1", "2", "3" };
        private readonly HashSet<string> viewValidCommands = new HashSet<string> { "0", "1", "2", "3", };


        public InputValidator()
        {
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


        public DateTime GetValidDateFromUser()
        {
            while (true)
            {
                Console.WriteLine("Enter enter date start time (yy-MM-dd):");
                string userInput = Console.ReadLine();

                if (CheckDateInput(userInput, out DateTime result))
                {
                    return result;
                }

                Console.WriteLine("Invalid date format please enter in format yy-MM-dd");
            }
        }

        public DateTime GetValidTimeFromUser() // refactor to remove writeline
        {
            while (true)
            {
                Console.WriteLine("Please enter the time (HH:mm):");
                string userInput = Console.ReadLine();

                if (CheckTimeInput(userInput, out DateTime result))
                {
                    return result;
                }

                Console.WriteLine("Invalid time format. Please try again.");
            }
        }
    }
}