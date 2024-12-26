using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CodingTracker.Business.ApplicationControls;
using CodingTracker.Common.BusinessInterfaces.ICodingSessionTimers;
using CodingTracker.Business.InputValidators;
using CodingTracker.Common.IApplicationControls;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.IUtilityServices;
using CodingTracker.Common.UtilityServices;
using CodingTracker.Data.Configurations;
using CodingTracker.Logging.ApplicationLoggers;
using CodingTracker.View.SessionGoalCountDownTimers;
using CodingTracker.Common.IInputValidationResults;
using CodingTracker.View.IMessageBoxManagers;
using CodingTracker.View.MessageBoxManagers;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.ErrorHandlers;
using CodingTracker.Data.DbContextService.CodingTrackerDbContexts;
using Microsoft.EntityFrameworkCore;
using CodingTracker.Data.Repositories.CodingSessionRepositories;
using CodingTracker.Common.DataInterfaces.ICodingTrackerDbContexts;
using CodingTracker.Common.IdGenerators;
using CodingTracker.Common.DataInterfaces.IUserCredentialRepositories;
using CodingTracker.Data.Repositories.UserCredentialRepositories;
using CodingTracker.Common.DataInterfaces.ICodingSessionRepositories;
using CodingTracker.Business.CodingSessionManagers;
using CodingTracker.Business.Authentication.AuthenticationServices;
using CodingTracker.Business.CodingSessionService.EditSessionPageContextManagers;
using CodingTracker.Business.CodingSessionService.UserIdServices;
using CodingTracker.Business.MainPageService;
using CodingTracker.View.FormService;
using CodingTracker.Common.BusinessInterfaces.IAuthenticationServices;
using CodingTracker.Common.BusinessInterfaces.ICodingSessionManagers;
using CodingTracker.Business.CodingSessionService;
using CodingTracker.Common.BusinessInterfaces;
using CodingTracker.Common.DataInterfaces;
using CodingTracker.Business.MainPageService.PanelColorControls;
using CodingTracker.Common.BusinessInterfaces.IPanelColourControls;
using CodingTracker.Business.MainPageService.PanelColourAssigners;


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
            Application.Run(loginPage);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();

            IConfiguration configuration = configurationBuilder.Build();

            var connectionString = configuration.GetSection("DatabaseConfig:ConnectionString").Value;

            services.AddSingleton<IConfiguration>(configuration)
                    .AddSingleton<IStartConfiguration, StartConfiguration>()  
                    .AddSingleton<IInputValidator, InputValidator>()
                    .AddSingleton<IApplicationLogger, ApplicationLogger>()
                    .AddSingleton<IUtilityService, UtilityService>()
                    .AddSingleton<IApplicationControl, ApplicationControl>()
                    .AddSingleton<IAuthenticationService, AuthenticationService>()
                    .AddSingleton<ISessionCalculator, SessionCalculator>()
                    .AddSingleton<IFormFactory, FormFactory>()
                    .AddSingleton<IFormController, FormController>()
                    .AddSingleton<IInputValidationResult, InputValidationResult>()
                    .AddSingleton<IMessageBoxManager, MessageBoxManager>()
                    .AddSingleton<IPanelColourControl, PanelColourControl>()
                    .AddSingleton<IErrorHandler, ErrorHandler>()
                    .AddSingleton<IFormSwitcher, FormSwitcher>()
                    .AddSingleton<ICodingSessionTimer, CodingSessionTimer>()
                    .AddSingleton<ICodingSessionCountDownTimer, CodingSessionCountDownTimer>()
                    .AddSingleton<IIdGenerators, IdGenerators>()
                    .AddSingleton<ICodingSessionRepository, CodingSessionRepository>()
                    .AddSingleton<ICodingTrackerDbContext, CodingTrackerDbContext>()
                    .AddSingleton<ICodingSessionManager, CodingSessionManager>()
                    .AddSingleton<IUserCredentialRepository , UserCredentialRepository>()
                    .AddSingleton<IPanelColourAssigner, PanelColourAssigner>()


                    .AddSingleton<EditSessionPageContextManager>()
                    .AddSingleton<UserIdService , UserIdService>()


                    // Transient services.
                    .AddTransient<ISessionGoalCountDownTimer, SessionGoalCountdownTimer>()
                    .AddTransient<LoginPage>()
                    .AddTransient<MainPage>()
                    .AddTransient<CodingSessionPage>()
                    .AddTransient<EditSessionPage>()
                    .AddTransient<CodingSessionTimerForm>()
                    .AddTransient<CreateAccountPage>()

                    .AddDbContext<CodingTrackerDbContext>(options =>
                    options.UseNpgsql(connectionString), ServiceLifetime.Scoped).AddScoped<ICodingTrackerDbContext, CodingTrackerDbContext>();



            var startConfiguration = services.BuildServiceProvider()
                                             .GetRequiredService<IStartConfiguration>();
            startConfiguration.LoadConfiguration();
        }
    }
}
