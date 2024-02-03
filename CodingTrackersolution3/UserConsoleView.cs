using CodingTracker.IInputvalidator;
using System;
using IConsoleView = CodingTracker.IUserConsoleView.IUserConsoleView;
using IDbManager = CodingTracker.IDatabaseManager.IDatabaseManager;
using ICrud = CodingTracker.ICRUD.ICRUD;
using System.Globalization;

//To do
// Add logic for validating input in consoleview or inputvalidator?

namespace CodingTracker.View.UserConsoleViews
{
    public class UserConsoleView : IConsoleView
    {

        private readonly IInputValidator _validator;
        private readonly IDbManager _databaseManager;
        private readonly ICrud _icrud;
        private readonly HashSet<string> startValidCommands = new HashSet<string> { "O", "1", "2", "3" }; //Define the specific HashSet collections or other variables directly within classes like UserConsoleView. Manage these variables locally within these classes.
        private readonly HashSet<string> viewValidCommands = new HashSet<string> { "0", "1", "2", "3" };



        public UserConsoleView(IInputValidator validator, IDbManager databaseManager, ICrud icrud)
        {
            _validator = validator;
            _databaseManager = databaseManager;
            _icrud = icrud;

        }
        public void DisplayStartOptions()
        {
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("\n");
            Console.WriteLine("1 to begin coding session");
            Console.WriteLine("2 to end the current coding session");
            Console.WriteLine("3 to view coding sessions");
            Console.WriteLine("O to exit program");
            Console.WriteLine("\nInput your choice");

            string startInput = Console.ReadLine();
            _validator.CheckStartInput(startInput);

        }



        public void DisplayViewOptions()
        {
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Choose an option to view your Sessions:");
            Console.WriteLine("1: View most recent sessions");
            Console.WriteLine("2: View all sessions");
            Console.WriteLine("3: Choose a session to view");
            Console.WriteLine("0: To exit program");
            Console.WriteLine("\nInput your choice:");

            string viewInput = Console.ReadLine().ToUpper();
            _validator.CheckViewInput(viewInput);

        }


        public string GetViewSpecificOptions() //show dates with coding sessions and then allow user to select one or use limit
        {
            string sessionId;

            foreach (var session in _icrud.ViewAllSession(CodingSession, true)) { }
            while (true)
            {
                Console.WriteLine("Enter the Session ID to view details, or 'exit' to return:");
                sessionId = Console.ReadLine();

                if (sessionId.ToLower() == "exit")
                    return null;

                if (_validator.CheckSessionId(sessionId) || _databaseManager.CheckSessionIdExist(sessionId))
                    break;
                else
                    Console.WriteLine("Invalid Session Id. Please enter a valid ID");
            }
            return sessionId;

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

                Console.WriteLine("Invalid date format please enter in format yy-MM-dd";
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


        public bool CheckDateInput(string input, out DateTime result)
        {
            return DateTime.TryParseExact(input, "yy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }

        public bool CheckTimeInput(string input, out DateTime result)
        {
            return DateTime.TryParseExact(input, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }

    }
}
