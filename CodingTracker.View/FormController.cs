using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.View;
using CodingTracker.View.FormFactories;
using CodingTracker.View.IFormControllers;
using CodingTrackerSolution;
using System.Diagnostics;
using CodingTracker.View.IFormFactories;

namespace CodingTracker.View.FormControllers
{
    public class FormController : IFormController
    {
        private readonly IApplicationLogger _appLogger;

        private Form currentForm;
        private IFormFactory _formFactory;

        public FormController(IApplicationLogger appLogger, IFormFactory formFactory)
        {
            _appLogger = appLogger;
            _formFactory = formFactory;

            this.currentForm = null;
        }

        public void ShowLoginPage()
        {
            var loginPage = _formFactory.CreateLoginPage();
            LogAndSwitchForm(loginPage, nameof(ShowLoginPage));
        }

        public void ShowMainPage()
        {
            var mainPage = _formFactory.CreateMainPage();
            LogAndSwitchForm(mainPage, nameof(ShowMainPage));
        }

        public void ShowCodingSessionPage()
        {
            var codingSessionPage = _formFactory.CreateCodingSessionPage();
            LogAndSwitchForm(codingSessionPage, nameof(ShowCodingSessionPage));
        }

        public void ShowEditSessionPage()
        {
            var editSessionPage = _formFactory.CreateEditSessionPage();
            LogAndSwitchForm(editSessionPage, nameof(ShowEditSessionPage));
        }

        public void ShowViewSessionPage()
        {
            var viewSessionsPage = _formFactory.CreateViewSessionsPage();
            LogAndSwitchForm(viewSessionsPage, nameof(ShowViewSessionPage));
        }

        public void ShowSettingsPage()
        {
            var settingsPage = _formFactory.CreateSettingsPage();
            LogAndSwitchForm(settingsPage, nameof(ShowSettingsPage));
        }

        public CreateAccountPage ShowCreateAccountPage()
        {
            var createAccountPage = _formFactory.CreateAccountPage();
            LogAndSwitchForm(createAccountPage, nameof(ShowCreateAccountPage));
            return createAccountPage; // Return the instance for callback setup
        }
        public void LogAndSwitchForm(Form newForm, string methodName)
        {
            using (var activity = new Activity(methodName).Start())
            {
                _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}, CurrentForm: {currentForm?.Name}, NewForm: {newForm.Name}");

                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    if (currentForm != null)
                    {
                        currentForm.Hide();
                        _appLogger.Info($"Hid form: {currentForm.Name}. TraceID: {activity.TraceId}");
                        currentForm.Dispose();
                    }
                    currentForm = newForm;
                    currentForm.Show();
                    stopwatch.Stop();
                    _appLogger.Info($"Showed form: {newForm.Name}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"Failed to switch forms in {methodName}. Error: {ex.Message}. TraceID: {Activity.Current?.TraceId}");
                }
            }



        }
    }
}