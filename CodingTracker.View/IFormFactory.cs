
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
        CreateAccountPage CreateAccountPage();
        CodingSessionTimerForm CreateCodingSessionTimer();




    }
}
