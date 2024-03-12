using CodingTracker.Common.ILoginManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodingTracker.View
{
    public partial class LoginPage : Form
    {
        private readonly ILoginManager _loginManager;
        public LoginPage(ILoginManager loginManager)
        {
            _loginManager = loginManager;
            InitializeComponent();
        }







        private void loginPageLoginButton_Click(object sender, EventArgs e)
        {
            string username = loginPageUsernameTextbox.Text;
            string password = loginPagePasswordTextbox.Text;

            var loginCredentials = _loginManager.ValidateLogin(username, password);

            if (loginCredentials != null)
            {
                MessageBox.Show("Login successful.");
            }
            else
            {
                loginPageErrorTextbox.Visible = true;
                loginPageErrorTextbox.Text = "Login failed. Please check your username and password.";

            }
        }

        private void loginPageExitButton_Click(object sender, EventArgs e)
        {

        }
    }
}
