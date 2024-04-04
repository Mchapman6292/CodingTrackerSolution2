using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodingTracker.Common.IApplicationControls;
using CodingTracker.View.IFormSwitchers;
using CodingTracker.View.IFormControllers;

namespace CodingTracker.View
{
    public partial class EditSessionPage : Form
    {
        private readonly IApplicationControl _appControl;
        private readonly IFormSwitcher _formSwitcher;
        private readonly IFormController _formController;
        public EditSessionPage(IApplicationControl appControl, IFormSwitcher formSwitcher)
        {
            InitializeComponent();
            _appControl = appControl;
            _formSwitcher = formSwitcher;
        }

        private void EditSessionPage_Load(object sender, EventArgs e)
        {

        }

        private void MainPageExitControlBox_Click(object sender, EventArgs e)
        {
            _formController.CloseCurrentForm();
        }
    }
}
