using Microsoft.Extensions.DependencyInjection;
using CodingTracker.View;
using CodingTracker.Common.IAuthtenticationServices;
using CodingTracker.Common.IApplicationLoggers;
using System.Diagnostics;
using CodingTracker.View.FormFactories;

namespace CodingTracker.View.FormFactories
{
    public interface IFormFactory
    {
        T CreateForm<T>(string methodName) where T : class;
        LoginPage CreateLoginPage();
        MainPage CreateMainPage();
        CodingSessionPage CreateCodingSessionPage();
        EditSessionPage CreateEditSessionPage();
        CreateAccountPage CreateAccountPage();
        CodingSessionTimerForm CreateCodingSessionTimer();

    }

    public class FormFactory : IFormFactory
    {

        private IServiceProvider _serviceProvider;
        private IApplicationLogger _appLogger;
        private MainPage _mainPageInstance;

        public  FormFactory(IServiceProvider serviceProvider,IApplicationLogger appLogger)
        {
            _serviceProvider = serviceProvider;
            _appLogger = appLogger;
        }

        public T CreateForm<T>(string methodName) where T : class
        {
            using (var activity = new Activity(methodName).Start())
            {
                _appLogger.Info($"Starting {methodName}. TraceID: {activity.TraceId}");
                try
                {
                    var page = _serviceProvider.GetRequiredService<T>();
                    _appLogger.Info($"Created {typeof(T).Name} successfully. TraceID: {activity.TraceId}");
                    return page;
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"An error occurred during {methodName}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    throw;
                }
            }
        }

        public LoginPage CreateLoginPage()
        {
            return CreateForm<LoginPage>(nameof(CreateLoginPage));
        }

        public MainPage CreateMainPage()
        {
            if (_mainPageInstance == null)
            {
                _mainPageInstance = CreateForm<MainPage>(nameof(CreateMainPage));
            }
            return _mainPageInstance;
        }

        public CodingSessionPage CreateCodingSessionPage()
        {
            return CreateForm<CodingSessionPage>(nameof(CreateCodingSessionPage));
        }

        public EditSessionPage CreateEditSessionPage()
        {
            return CreateForm<EditSessionPage>(nameof(CreateEditSessionPage));
        }
        public CreateAccountPage CreateAccountPage() 
        {
            return CreateForm<CreateAccountPage>(nameof(CreateAccountPage));    
        }

        public CodingSessionTimerForm CreateCodingSessionTimer()
        {
            return CreateForm<CodingSessionTimerForm>(nameof(CreateCodingSessionTimer));
        }
    }
}
