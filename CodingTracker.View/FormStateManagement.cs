using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.View.FormStateManagements
{
    public class FormStateManagement
    {
        private Dictionary<Type, object> formStates = new Dictionary<Type, object>();

        public void SaveFormState<TForm>(TForm form, object state) where TForm : Form
        {
            formStates[typeof(TForm)] = state;
        }

        public object GetFormState<TForm>() where TForm : Form
        {
            formStates.TryGetValue(typeof(TForm), out var state);
            return state;
        }

    }
}
