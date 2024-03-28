using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.View;
using CodingTracker.View.FormFactories;
using CodingTracker.View.IFormControllers;
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
            try
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug("Starting ShowLoginPage.");

                var loginPage = _formFactory.CreateLoginPage();
                LogAndSwitchForm(loginPage, nameof(ShowLoginPage));

                stopwatch.Stop();
                _appLogger.Info($"ShowLoginPage completed in {stopwatch.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error in ShowLoginPage: {ex.Message}", ex);
                MessageBox.Show("An error occurred while opening the login Page. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }
        }

        public void ShowMainPage()
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug("Starting ShowMainPage.");

                var mainPage = _formFactory.CreateMainPage();
                LogAndSwitchForm(mainPage, nameof(ShowMainPage));

                stopwatch.Stop();
                _appLogger.Info($"ShowMainPage completed in {stopwatch.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error in ShowMainPage: {ex.Message}", ex);
                MessageBox.Show("An error occurred while opening the main Page. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ShowCodingSessionPage()
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug("Starting ShowCodingSessionPage.");

                var codingSessionPage = _formFactory.CreateCodingSessionPage();
                LogAndSwitchForm(codingSessionPage, nameof(ShowCodingSessionPage));

                stopwatch.Stop();
                _appLogger.Info($"ShowCodingSessionPage completed in {stopwatch.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error in ShowCodingSessionPage: {ex.Message}", ex);
                MessageBox.Show("An error occurred while opening the coding session Page. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ShowEditSessionPage()
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug("Starting ShowEditSessionPage.");

                var editSessionPage = _formFactory.CreateEditSessionPage();
                LogAndSwitchForm(editSessionPage, nameof(ShowEditSessionPage));

                stopwatch.Stop();
                _appLogger.Info($"ShowEditSessionPage completed in {stopwatch.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error in ShowEditSessionPage: {ex.Message}", ex);
                MessageBox.Show("An error occurred while opening the edit session Page. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ShowViewSessionPage()
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug("Starting ShowViewSessionPage.");

                var viewSessionsPage = _formFactory.CreateViewSessionsPage();
                LogAndSwitchForm(viewSessionsPage, nameof(ShowViewSessionPage));

                stopwatch.Stop();
                _appLogger.Info($"ShowViewSessionPage completed in {stopwatch.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error in ShowViewSessionPage: {ex.Message}", ex);
                MessageBox.Show("An error occurred while opening the view session Page. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw; 
            }
        }

        public void ShowSettingsPage()
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug("Starting ShowSettingsPage.");

                var settingsPage = _formFactory.CreateSettingsPage();
                LogAndSwitchForm(settingsPage, nameof(ShowSettingsPage));

                stopwatch.Stop();
                _appLogger.Info($"ShowSettingsPage completed in {stopwatch.ElapsedMilliseconds}ms.");
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error in ShowSettingsPage: {ex.Message}", ex);
                MessageBox.Show("An error occurred while opening the settings page Page. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw; 
            }
        }

        public CreateAccountPage ShowCreateAccountPage()
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                _appLogger.Debug("Starting ShowCreateAccountPage.");

                var createAccountPage = _formFactory.CreateAccountPage();
                LogAndSwitchForm(createAccountPage, nameof(ShowCreateAccountPage));

                stopwatch.Stop();
                _appLogger.Info($"ShowCreateAccountPage completed in {stopwatch.ElapsedMilliseconds}ms.");
                return createAccountPage;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error in ShowCreateAccountPage: {ex.Message}", ex);
                MessageBox.Show("An error occurred while opening the Create Account Page. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
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