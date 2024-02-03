
using System;
using CodingTracker.Common.IUserConsoleView;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Business.CodingSession;
using CodingTracker.Common.IInputValidator;
using CodingTracker.Common.ICRUDs;


//To do
// Add logic for validating input in consoleview or inputvalidator?

namespace CodingTracker.View.UserConsoleViews
{
    public class UserConsoleView : IUserConsoleView

    {

        private readonly IInputValidator _validator;
        private readonly IDatabaseManager _databaseManager;
        private readonly ICRUD _ICRUD;
        private readonly HashSet<string> startValidCommands = new HashSet<string> { "O", "1", "2", "3" }; //Define the specific HashSet collections or other variables directly within classes like UserConsoleView. Manage these variables locally within these classes.
        private readonly HashSet<string> viewValidCommands = new HashSet<string> { "0", "1", "2", "3" };



        public UserConsoleView(IInputValidator validator, IDatabaseManager databaseManager, ICRUD ICRUD)
        {
            _validator = validator;
            _databaseManager = databaseManager;
            _ICRUD = ICRUD;

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

            foreach (var session in _ICRUD.ViewAllSession(CodingSession, true)) { }
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
    }
}
}