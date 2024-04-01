using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.View.IFormSwitchers
{
    public interface IFormSwitcher
    {
        void SwitchToLoginPage();
        void SwitchToMainPage();
        void SwitchToCodingSessionPage();
        void SwitchToEditSessionPage();
        void SwitchToSettingsPage();
        void SwitchToViewSessionsPage();
        CreateAccountPage SwitchToCreateAccountPage();
        void SwitchToCodingSessionTimer();







    }
}
