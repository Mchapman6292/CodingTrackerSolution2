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
        void HandleAndShowForm<TForm>(Func<TForm> createForm, string methodName, bool closeCurrent = true) where TForm : Form;
        void ExecutePageAction(Action action, string methodName);
        void CloseCurrentForm();
        void DisplayForm<TForm>(TForm newForm) where TForm : Form;
        void CloseTargetForm(Form targetForm);
    }
}