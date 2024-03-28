using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CodingTracker.Business.ApplicationControls;
using CodingTracker.Business.CodingGoals;
using CodingTracker.Business.CodingSessions;
using CodingTracker.Common.InputValidators;
using CodingTracker.Common.IApplicationControls;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.ICodingGoals;
using CodingTracker.Common.ICodingSessions;
using CodingTracker.Common.ICredentialManagers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.ILoginManagers;
using CodingTracker.Common.IStartConfigurations;
using CodingTracker.Common.IUserCredentialDTOs;
using CodingTracker.Common.IUtilityServices;
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.UtilityServices;
using CodingTracker.Data.Configurations;
using CodingTracker.Data.CredentialManagers;
using CodingTracker.Data.DatabaseManagers;
using CodingTracker.Data.LoginManagers;
using CodingTracker.Logging.ApplicationLoggers;
using CodingTracker.View.FormFactories;
using CodingTracker.View.IFormFactories;
using CodingTracker.View.IFormControllers;
using CodingTracker.View.FormControllers;
using CodingTracker.View.SessionGoalCountDownTimers;
using CodingTracker.Common.ISessionGoalCountDownTimers;
using CodingTracker.Common.IInputValidationResults;
using CodingTracker.View.IMessageBoxManagers;
using CodingTracker.View.MessageBoxManagers;
using System.ComponentModel.DataAnnotations;
using CodingTracker.Common.InputValidationResults;
using CodingTracker.Business.PanelColorControls;
using CodingTracker.Common.IPanelColorControls;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.ErrorHandlers;

/// To do
/// Change get validDate & Time inputvalidator
/// Consistent appraoch to DTO
/// Add event logic to show account created succesfully in loginpage.
/// Check all methods end stopwatch timing when error is thrown
/// Review database methods to add more sql lite exceptions. 

namespace CodingTracker.View.Program
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);



            using var serviceProvider = services.BuildServiceProvider();
            ApplicationConfiguration.Initialize();

            var formFactory = serviceProvider.GetRequiredService<IFormFactory>();
            var loginPage = formFactory.CreateLoginPage();
            var dbManager = serviceProvider.GetRequiredService<IDatabaseManager>();
            dbManager.EnsureDatabaseForUser();
            dbManager.CreateTableIfNotExists();
            Application.Run(loginPage);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();

            IConfiguration configuration = configurationBuilder.Build();

            services.AddSingleton<IConfiguration>(configuration)
                    .AddSingleton<IStartConfiguration, StartConfiguration>()  
                    .AddSingleton<IInputValidator, InputValidator>()
                    .AddSingleton<IDatabaseManager, DatabaseManager>()
                    .AddSingleton<IUtilityService, UtilityService>()
                    .AddSingleton<IApplicationControl, ApplicationControl>()
                    .AddSingleton<ILoginManager, LoginManager>()
                    .AddSingleton<IUserCredentialDTO, UserCredentialDTO>()
                    .AddSingleton<ICredentialManager, CredentialManager>()
                    .AddSingleton<IApplicationLogger, ApplicationLogger>()
                    .AddSingleton<IFormFactory, FormFactory>()
                    .AddSingleton<ICodingSession, CodingSession>()
                    .AddSingleton<ICodingGoal, CodingGoal>()
                    .AddSingleton<IFormController, FormController>()
                    .AddSingleton<ISessionGoalCountDownTimer, SessionGoalCountDownTimer>()
                    .AddSingleton<IInputValidationResult, InputValidationResult>()
                    .AddSingleton<IMessageBoxManager, MessageBoxManager>()
                    .AddSingleton<IPanelColorControl, PanelColorControl>()
                    .AddSingleton<IErrorHandler, ErrorHandler>()
                    .AddTransient<LoginPage>()
                    .AddSingleton<MainPage>()
                    .AddSingleton<CodingSessionPage>()
                    .AddSingleton<EditSessionPage>()
                    .AddSingleton<ViewSessionsPage>()
                    .AddSingleton<CreateAccountPage>();

            var startConfiguration = services.BuildServiceProvider()
                                             .GetRequiredService<IStartConfiguration>();
            startConfiguration.LoadConfiguration();
        }
    }
}
