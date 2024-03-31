using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodingTracker.View.IFormControllers
{
    public interface IFormController
    {
        void HandleAndShowForm(Func<Form> createForm, string methodName, bool closeCurrent = true);
        void ExecutePageAction(Action action, string methodName);
        void CloseCurrentForm();
        void DisplayForm(Form newForm);
        void CloseTargetForm(Form targetForm);
    }
}