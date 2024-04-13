using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.View.FormControllers;
using CodingTracker.View.FormFactories;

namespace CodingTracker.View.FormControllers
{

    public interface IFormController
    {
        void HandleAndShowForm<TForm>(Func<TForm> createForm, string methodName, bool closeCurrent = true) where TForm : Form;
        void ExecutePageAction(Action action, string methodName);
        void CloseCurrentForm();
        void DisplayForm<TForm>(TForm newForm) where TForm : Form;
        void CloseTargetForm(Form targetForm);
    }
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

        public void HandleAndShowForm<TForm>(Func<TForm> createForm, string methodName, bool closeCurrent = true) where TForm : Form // Handles the logic for closing forms & implementing error handling logic via ExecutePageAction
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
                currentForm.Hide(); 
            }
        }

        public void DisplayForm<TForm>(TForm newForm) where TForm : Form
        {
            if (newForm == null)
            {
                _appLogger.Error($"Attempted to display a null form in {nameof(DisplayForm)}.");
                throw new ArgumentNullException(nameof(newForm), "New form is null.");
            }

            currentForm = newForm;
            currentForm.Show();
            _appLogger.Info($"Opened {newForm.Name}");
        }



        public void CloseTargetForm(Form targetForm) // Probably not needed but keep for now. 
        {
            if (targetForm == null) return; 

            if (targetForm.Visible)
            {
                _appLogger.Info($"Closing form: {targetForm.Name}");
                targetForm.Invoke(new Action(() =>
                {
                    targetForm.Hide();
                    targetForm.Dispose(); 
                }));
            }
        }
    }
}
