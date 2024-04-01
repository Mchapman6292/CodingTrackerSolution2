
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.View.IFormFactories
{
    public interface IFormFactory
    {
        T CreateForm<T>(string methodName) where T : class;
         LoginPage CreateLoginPage();
         MainPage CreateMainPage();
         CodingSessionPage CreateCodingSessionPage();
         EditSessionPage CreateEditSessionPage();
         SettingsPage CreateSettingsPage();
         ViewSessionsPage CreateViewSessionsPage();
        CreateAccountPage CreateAccountPage();
        CodingSessionTimer CreateCodingSessionTimer();




    }
}
