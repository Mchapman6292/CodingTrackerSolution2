using CodingTracker.View.FormControllers;
using CodingTracker.View.FormFactories;

namespace CodingTracker.View.FormSwitchers
{

    public interface IFormSwitcher
    {
        void SwitchToLoginPage();
        void SwitchToMainPage();
        void SwitchToCodingSessionPage();
        void SwitchToEditSessionPage();
        CreateAccountPage SwitchToCreateAccountPage();
        void SwitchToCodingSessionTimer();
    }


    public class FormSwitcher : IFormSwitcher
        {
            private readonly IFormController _formController;
            private readonly IFormFactory _formFactory;

            public FormSwitcher(IFormController formController, IFormFactory formFactory)
            {
                _formController = formController;
                _formFactory = formFactory;
            }

            private void SwitchToForm<T>(Func<T> createForm) where T : Form
            {
                _formController.HandleAndShowForm(() => createForm() as Form, typeof(T).Name, true);
            }

            public void SwitchToLoginPage()
            {
                SwitchToForm(_formFactory.CreateLoginPage);
            }

            public void SwitchToMainPage()
            {
                SwitchToForm(_formFactory.CreateMainPage);
            }

            public void SwitchToCodingSessionPage()
            {
                SwitchToForm(_formFactory.CreateCodingSessionPage);
            }

            public void SwitchToEditSessionPage()
            {
                SwitchToForm(_formFactory.CreateEditSessionPage);
            }



            public CreateAccountPage SwitchToCreateAccountPage() // This is implemented to return an instance of CreateAccountPage so that the AccountCreatedCallback can be triggered. This allows for the Account Created message to be displayed on the LoginPage once a user account has been created. 
            {
                var createAccountPage = _formFactory.CreateAccountPage();
                _formController.HandleAndShowForm(() => createAccountPage, nameof(CreateAccountPage), true);
                return createAccountPage;
            }

            public void SwitchToCodingSessionTimer()
            {
                SwitchToForm(_formFactory.CreateCodingSessionTimer);
            }
        }
    }
