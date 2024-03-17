using Microsoft.Extensions.DependencyInjection;
using CodingTracker.View;
using CodingTracker.Common.ILoginManagers;
using CodingTracker.Common.IApplicationLoggers;
using CodingTrackerSolution;
using System.Diagnostics;
using CodingTracker.View.IFormFactories;

namespace CodingTracker.View.FormFactories
{
    public class FormFactory : IFormFactory
    {

        private IServiceProvider _serviceProvider;
        private IApplicationLogger _appLogger;

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
            return CreateForm<MainPage>(nameof(CreateMainPage));
        }

        public CodingSessionPage CreateCodingSessionPage()
        {
            return CreateForm<CodingSessionPage>(nameof(CreateCodingSessionPage));
        }

        public EditSessionPage CreateEditSessionPage()
        {
            return CreateForm<EditSessionPage>(nameof(CreateEditSessionPage));
        }

        public SettingsPage CreateSettingsPage()
        {
            return CreateForm<SettingsPage>(nameof(CreateSettingsPage));
        }

        public ViewSessionsPage CreateViewSessionsPage()
        {
            return CreateForm<ViewSessionsPage>(nameof(CreateViewSessionsPage));
        }

    }
}
