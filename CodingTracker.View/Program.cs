using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CodingTracker.Business.InputValidators;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Data.DatabaseManagers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.View.UserConsoleViews;
using CodingTracker.Common.IUserConsoleView;
using CodingTracker.Data.CRUDs;
using CodingTracker.Common.ICRUDs;


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
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration JSONconfiguration = configurationBuilder.Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(JSONconfiguration)
                .AddSingleton<IInputValidator, InputValidator>()
                .AddSingleton<IDatabaseManager, DatabaseManager>()
                .AddSingleton<IUserConsoleView, UserConsoleView>() 
                .AddSingleton<ICRUD, CRUD>()
                .BuildServiceProvider();


            ApplicationConfiguration.Initialize();

            Application.Run(new Form1()); 
        }
    }
}
