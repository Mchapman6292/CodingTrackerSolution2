using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using DbManager = CodingTracker.Data.DatabaseManagers.DatabaseManager;
using IDbManager = CodingTracker.Data.IDatabaseManagers.IDatabaseManager;
using Validator = CodingTracker.Business.InputValidators.InputValidator;
using IValidator = CodingTracker.Business.IInputValidators.IInputValidator;
using Crud = CodingTracker.Data.CRUDs.CRUD;
using ICrud = CodingTracker.Data.ICRUDs.ICRUD;
using Config = CodingTracker.Data.Configurations.Configuartion;
using IConfig = CodingTracker.Data.IAppConfigs.IAppConfig;
using UCredentialDTO = CodingTracker.DTO.UserCredentialDTOs.UserCredentialDTO;
using CSessionDTO = CodingTracker.DTO.CodingSessionDTOs.CodingSessionDTO;
using System.Configuration;



namespace CodingTracker.View.Program
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();


            IConfiguration JSONconfiguration = configurationBuilder.Build();

            var serviceProvider = new ServiceCollection()
            .AddSingleton<IConfiguration>(JSONconfiguration)//Iconfiguration instance(.net)
                .AddSingleton<IValidator, Validator>() // Tells di container to give the instance of Inputvalidator
                .AddSingleton<IDbManager, DbManager>()
                .AddSingleton<IConfig, Config>()
                .AddSingleton<ICrud, Crud>()
                .AddSingleton<CSessionDTO>()
                .AddSingleton<UCredentialDTO>()

                .BuildServiceProvider();



            //use the ServiceCollection to register services
            //Singleton: The same instance is used across the application. Created only once during the application's lifetime.
            //Scoped: A new instance is created for each request or each time a new scope is created.Commonly used in web applications for per - request dependencies.
            //Transient: A new instance is created every time a service is requested.

            //Best practice to depend on abstractions rather than concrete implementations. 

            //register your services in the DI container using services.AddSingleton<Interface, Implementation>()
            //Once services are registered, you can retrieve them using serviceProvider.GetService<Interface>()
            //The most common way to use DI in .NET is through constructor injection. In your classes that require dependencies, you define a constructor that takes these dependencies as parameters.
            // There is no chain of dependencies as each class has its own container passed to it
        }
    }
}