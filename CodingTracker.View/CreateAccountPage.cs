using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodingTracker.Common.ICredentialManagers;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Data.CredentialManagers;
using CodingTracker.Common.IInputValidators;
using CodingTracker.View.IFormControllers;
using System.Diagnostics;
using CodingTracker.View;

namespace CodingTracker.View
{

    public partial class CreateAccountPage : Form
    {
        private readonly ICredentialManager _credentialManager;
        private readonly IInputValidator _inputValidator;
        private readonly IApplicationLogger _appLogger;
        private readonly IFormController _formController;
        public event EventHandler AccountCreatedSuccessfully;

        public CreateAccountPage(ICredentialManager credentialManager, IInputValidator inputValidator, IApplicationLogger appLogger, IFormController formController)
        {
            InitializeComponent();
            _credentialManager = credentialManager;
            _inputValidator = inputValidator;
            _appLogger = appLogger;
            _formController = formController;
        }

        private void DisplayErrorMessage(string message)
        {
            CreateAccountPageErrorTextBox.Text = message; 
        }


        private void CreateAccountPageCreateAccountButton_Click(object sender, EventArgs e)
        {
            Stopwatch overallStopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(CreateAccountPageCreateAccountButton)).Start())
            {
                _appLogger.Debug($"Starting {nameof(CreateAccountPageCreateAccountButton)}. TraceID: {activity.TraceId}");

                string username = CreateAccountPageUsernameTextbox.Text;
                string password = CreateAccountPasswordTextbox.Text;

                var usernameResult = _inputValidator.ValidateUsername(username);
                var passwordResult = _inputValidator.ValidatePassword(password);    

                if ((usernameResult.IsValid && passwordResult.IsValid))
                {
                    try
                    {
                        _credentialManager.CreateAccount(username, password);
                        _appLogger.Info($"Account creation successful for user: {username}. Total Duration: {overallStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                        AccountCreatedSuccessfully?.Invoke(this, EventArgs.Empty);
                        _formController.ShowLoginPage();
                    }
                    catch (Exception ex)
                    {
                        _appLogger.Error($"Account creation failed for user: {username}. Error: {ex.Message}. Total Duration: {overallStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                        DisplayErrorMessage(ex.Message);
                    }
                }
                else
                {
                    // Only triggers if the username or password does not match requirements, displays all error messages.
                    var errorMessages = $"{usernameResult.GetAllErrorMessages()}\n{passwordResult.GetAllErrorMessages()}";
                    _appLogger.Warning($"Validation failed for username or password. Total Duration: {overallStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    DisplayErrorMessage(errorMessages);
                    
                }
            }
            overallStopwatch.Stop();
        }
    }
}