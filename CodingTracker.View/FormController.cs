using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.View.IFormControllers;
using CodingTracker.View.IFormFactories;

namespace CodingTracker.View.FormControllers
{
    public class FormController : IFormController
    {
        private readonly IApplicationLogger _appLogger;
        private Form currentForm;
        private readonly IFormFactory _formFactory;
        private readonly IErrorHandler _errorHandler;

        public FormController(IApplicationLogger appLogger, IFormFactory formFactory, IErrorHandler errorHandler)
        {
            _appLogger = appLogger;
            _formFactory = formFactory;
            _errorHandler = errorHandler;
        }

        public void HandleAndShowForm(Func<Form> createForm, string methodName, bool closeCurrent = true)
        {
            ExecutePageAction(() =>
            {
                var newForm = createForm();
                if (closeCurrent)
                {
                    CloseCurrentForm();
                }
                DisplayForm(newForm);
            }, methodName);
        }

        public void ExecutePageAction(Action action, string methodName)
        {
            _errorHandler.CatchErrorsAndLogWithStopwatch(action, methodName, false); // false indicates this is not a database operation
        }

        public void CloseCurrentForm()
        {
            if (currentForm != null)
            {
                currentForm.Close();
                currentForm.Dispose();
                currentForm = null;
            }
        }

        public void DisplayForm(Form newForm)
        {
            currentForm = newForm;
            currentForm.Show();
            _appLogger.Info($"Opened {newForm.Name}");
        }

        public void CloseTargetForm(Form targetForm)
        {
            if (targetForm == null) return; 

            if (targetForm.Visible)
            {
                _appLogger.Info($"Closing form: {targetForm.Name}");
                targetForm.Invoke(new Action(() =>
                {
                    targetForm.Hide(); // Hide the form first.
                    targetForm.Dispose(); // Then dispose of it to release resources.
                }));
            }
        }
    }
}
