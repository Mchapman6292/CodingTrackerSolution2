using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CodingTracker.Business.ApplicationControls;
using CodingTracker.Business.CodingSessions;
using CodingTracker.Common.InputValidators;
using CodingTracker.Common.IApplicationControls;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.ICodingSessions;
using CodingTracker.Common.ICredentialManagers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.ILoginManagers;
using CodingTracker.Common.IStartConfigurations;
using CodingTracker.Common.IUtilityServices;
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.UtilityServices;
using CodingTracker.Data.Configurations;
using CodingTracker.Data.CredentialManagers;
using CodingTracker.Data.DatabaseManagers;
using CodingTracker.Common.IAuthenticationServices;
using CodingTracker.Logging.ApplicationLoggers;
using CodingTracker.View.FormFactories;
using CodingTracker.View.FormControllers;
using CodingTracker.View.SessionGoalCountDownTimers;
using CodingTracker.Common.ISessionGoalCountDownTimers;
using CodingTracker.Common.IInputValidationResults;
using CodingTracker.View.IMessageBoxManagers;
using CodingTracker.View.MessageBoxManagers;
using CodingTracker.Common.InputValidationResults;
using CodingTracker.Business.PanelColorControls;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.ErrorHandlers;
using CodingTracker.View.FormSwitchers;
using CodingTracker.Business.CodingSessionTimers;
using CodingTracker.Common.ICodingSessionTimers;
using CodingTracker.Business.CodingSessionCountDownTimers;
using CodingTracker.Common.CodingSessionDTOManagers;
using CodingTracker.Business.SessionCalculators;
using CodingTracker.Common.UserCredentialDTOManagers;
using CodingTracker.Data.QueryBuilders;
using CodingTracker.Common.IQueryBuilders;
using CodingTracker.Data.NewDatabaseReads;
using CodingTracker.Common.INewDatabaseReads;
using CodingTracker.Data.EntityContexts;
using Microsoft.EntityFrameworkCore;
using CodingTracker.Common.DataInterfaces.CodingSessionRepository;
using CodingTracker.Common.DataInterfaces.IEntityContexts;
using CodingTracker.Common.IdGenerators;
using CodingTracker.Data.Interfaces.IUserCredentialRepository;
using CodingTracker.Data.Repositories.UserCredentialRepository;
using CodingTracker.Common.Interfaces.ICodingSessionRepository;
/// To do
/// Change get validDate & Time inputvalidator
/// Consistent appraoch to DTO
/// Add event logic to show account created succesfully in loginpage.
/// Check all methods end stopwatch timing when error is thrown
/// Review database methods to add more sql lite exceptions. 
/// Review all methods were thrown is used. 
/// Centralize errorboxmessage logic.
/// Add tests to ensure that the labels and panel days correspond.
/// Change CatchErrorsAndLogWithStopwatch so that it does not call the method itself. 
/// Logic for remember me to read stored password from sql lite db/other
/// get user id & session id both use most recent login. CHANGE

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
                    .AddSingleton<IApplicationLogger, ApplicationLogger>()
                    .AddSingleton<IUserCredentialDTOManager, UserCredentialDTOManager>()
                    .AddSingleton<ICredentialManager, CredentialManager>()
                    .AddSingleton<INewDatabaseRead, NewDatabaseRead>()
                    .AddSingleton<IUtilityService, UtilityService>()
                    .AddSingleton<IApplicationControl, ApplicationControl>()
                    .AddSingleton<IAuthenticationService, AuthenticationService>()
                    .AddSingleton<ISessionCalculator, SessionCalculator>()
                    .AddSingleton<IFormFactory, FormFactory>()
                    .AddSingleton<ISessionLogic, SessionLogic>()
                    .AddSingleton<IFormController, FormController>()
                    .AddSingleton<IInputValidationResult, InputValidationResult>()
                    .AddSingleton<IMessageBoxManager, MessageBoxManager>()
                    .AddSingleton<IPanelColorControl, PanelColorControl>()
                    .AddSingleton<IErrorHandler, ErrorHandler>()
                    .AddSingleton<IFormSwitcher, FormSwitcher>()
                    .AddSingleton<ICodingSessionTimer, CodingSessionTimer>()
                    .AddSingleton<ICodingSessionCountDownTimer, CodingSessionCountDownTimer>()
                    .AddSingleton<IQueryBuilder, QueryBuilder>()
                    .AddSingleton<IIdGenerators, IdGenerators>()
                    .AddSingleton<IEntityContext, EntityContext>()
                    .AddSingleton<IUserCredentialRepository, UserCredentialRepository>()
                    .AddSingleton<ICodingSessionRepository, CodingSessionRepository>()
                    .AddSingleton<IEntityContext, EntityContext>()


                    // Transient services.
                    .AddTransient<ISessionGoalCountDownTimer, SessionGoalCountdownTimer>()
                    .AddTransient<LoginPage>()
                    .AddTransient<MainPage>()
                    .AddTransient<CodingSessionPage>()
                    .AddTransient<EditSessionPage>()
                    .AddTransient<CodingSessionTimerForm>()
                    .AddTransient<CreateAccountPage>()

                    .AddDbContext<EntityContext>(options =>
                        options.UseSqlite("Data Source=path_to_your_database.db"));



            var startConfiguration = services.BuildServiceProvider()
                                             .GetRequiredService<IStartConfiguration>();
            startConfiguration.LoadConfiguration();
        }
    }
}
