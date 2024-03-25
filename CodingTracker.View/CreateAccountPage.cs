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
using System.Diagnostics;

namespace CodingTracker.View
{

    public partial class CreateAccountPage : Form
    {
        private readonly ICredentialManager _credentialManager;
        private readonly IInputValidator _inputValidator;
        private readonly IApplicationLogger _appLogger;

        public CreateAccountPage(ICredentialManager credentialManager, IInputValidator inputValidator, IApplicationLogger appLogger)
        {
            InitializeComponent();
            _credentialManager = credentialManager;
            _inputValidator = inputValidator;
            _appLogger = appLogger;
        }

        private void CreateAccountPageCreateAccountButton_Click(object sender, EventArgs e)
        {
            Stopwatch overallStopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(CreateAccountPageCreateAccountButton)).Start())
            {
                _appLogger.Debug($"Starting {nameof(CreateAccountPageCreateAccountButton)}. TraceID: {activity.TraceId}");

                string username = CreateAccountPageUsernameTextbox.Text;
                string password = CreateAccountPasswordTextbox.Text;

                if (_inputValidator.ValidateUsername(username) && _inputValidator.ValidatePassword(password))
                {
                    try
                    {
                        _credentialManager.CreateAccount(username, password);
                        _appLogger.Info($"Account creation successful for user: {username}. Total Duration: {overallStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }
                    catch (Exception ex)
                    {
                        _appLogger.Error($"Account creation failed for user: {username}. Error: {ex.Message}. Total Duration: {overallStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                        // Handle error
                    }
                }
                else
                {
                    _appLogger.Warning($"Validation failed for username or password. Total Duration: {overallStopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    // Inform the user that the username or password is invalid
                }
            }
            overallStopwatch.Stop();
        }
    }
}