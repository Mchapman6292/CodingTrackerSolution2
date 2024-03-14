using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CodingTracker.Business.InputValidators;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Data.DatabaseManagers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IStartConfigurations;
using CodingTracker.Data.Configurations;
using CodingTracker.Common.IUtilityServices;
using CodingTracker.Common.UtilityServices;
using CodingTracker.Business.ApplicationControls;
using CodingTracker.Common.IApplicationControls;
using CodingTracker.Data.LoginManagers;
using CodingTracker.Common.ILoginManagers;
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.IUserCredentialDTOs;
using CodingTracker.Data.CredentialStorage;
using CodingTracker.Common.ICredentialStorage;
using CodingTracker.Logging.ApplicationLoggers;
using CodingTracker.Common.IApplicationLoggers;

//To do
// refactor classes to use CodingSessionDTO as parameter
//


namespace CodingTracker.View.Program
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false) // reloadOnChange set to true for dynamic construction, when reading from json file(static) set to false.
                .AddEnvironmentVariables();

            IConfiguration JSONconfiguration = configurationBuilder.Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(JSONconfiguration)
                .AddSingleton<IStartConfiguration, StartConfiguration>()
                .AddSingleton<IInputValidator, InputValidator>()
                .AddSingleton<IDatabaseManager, DatabaseManager>()
                .AddSingleton<IUtilityService,UtilityService>()
                .AddSingleton<IApplicationControl, ApplicationControl>()
                .AddSingleton<ILoginManager, LoginManager>()
                .AddSingleton<IUserCredentialDTO, UserCredentialDTO>()
                .AddSingleton<ICredentialStorage, CredentialStorage>()
                .AddSingleton<IApplicationLogger, ApplicationLogger>()
                .BuildServiceProvider();


            ApplicationConfiguration.Initialize();

            Application.Run(new LoginPage()); 
        }
    }
}
