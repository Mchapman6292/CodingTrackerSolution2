using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodingTracker.View.IFormControllers
{
    public interface IFormController
    {

        void LogAndSwitchForm(Form newForm, string methodName);
        void ShowLoginPage();
        void ShowMainPage();
        void ShowCodingSessionPage();
        void ShowEditSessionPage();
        void ShowViewSessionPage();
        void ShowSettingsPage();
        void ShowCreateAccountPage();

    }
}