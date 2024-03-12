using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.View;
using CodingTracker.View.IFormControllers;
using CodingTrackerSolution;
using System.Diagnostics;

public class FormController : IFormController
{
    private readonly IApplicationLogger _appLogger;
    private CodingSessionPage codingSessionPage;
    private LoginPage loginPage;
    private MainPage mainPage;
    private ViewSessionsPage viewSessionsPage;
    private EditSessionPage editSessionPage;
    private SettingsPage settingsPage;
    private Form currentForm;

    public FormController(IApplicationLogger appLogger, CodingSessionPage codingSessionPage, LoginPage loginPage, MainPage mainPage, ViewSessionsPage viewSessionsPage, EditSessionPage editSessionPage, SettingsPage settingsPage)
    {
        _appLogger = appLogger;
        this.codingSessionPage = codingSessionPage;
        this.loginPage = loginPage;
        this.mainPage = mainPage;
        this.viewSessionsPage = viewSessionsPage;
        this.editSessionPage = editSessionPage;
        this.settingsPage = settingsPage;
        this.currentForm = null;
    }

    public void ShowLoginPage()
    {
        LogAndSwitchForm(loginPage, nameof(ShowLoginPage));
    }

    public void ShowMainPage()
    {
        LogAndSwitchForm(mainPage, nameof(ShowMainPage));
    }

    public void ShowCodingSessionPage()
    {
        LogAndSwitchForm(codingSessionPage, nameof(ShowCodingSessionPage));
    }

    public void ShowEditSessionPage()
    {
        LogAndSwitchForm(editSessionPage, nameof(ShowEditSessionPage));
    }

    public void ShowViewSessionPage()
    {
        LogAndSwitchForm(viewSessionsPage, nameof(ShowEditSessionPage));
    }

    public void ShowSettingsPage()
    {
        LogAndSwitchForm(settingsPage, nameof(ShowSettingsPage));
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