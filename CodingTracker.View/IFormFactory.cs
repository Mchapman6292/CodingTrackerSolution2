using CodingTrackerSolution;
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
        public LoginPage CreateLoginPage();
        public MainPage CreateMainPage();
        public CodingSessionPage CreateCodingSessionPage();
        public EditSessionPage CreateEditSessionPage();
        public SettingsPage CreateSettingsPage();
        public ViewSessionsPage CreateViewSessionsPage();
        public CreateAccountPage CreateAccountPage();



    }
}
