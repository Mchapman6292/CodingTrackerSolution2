using CodingTracker.Common.ILoginManagers;

namespace CodingTracker.View
{
    public partial class LoginPage : Form
    {
        private readonly ILoginManager _loginManager;
        public LoginPage(ILoginManager loginManager)
        {
            InitializeComponent();
            _loginManager = loginManager;
        }

        private void InitializeComponent()
        {
            loginUsernameLabel = new Label();
            loginPasswordLabel = new Label();
            loginUsernameTextbox = new TextBox();
            loginPasswordTextbox = new TextBox();
            LoginPageLoginButton = new Button();
            loginPageExitButton = new Button();
            label1 = new Label();
            LoginPageErrorLabel = new Label();
            SuspendLayout();
            // 
            // loginUsernameLabel
            // 
            loginUsernameLabel.Location = new Point(578, 301);
            loginUsernameLabel.Name = "loginUsernameLabel";
            loginUsernameLabel.Size = new Size(104, 25);
            loginUsernameLabel.TabIndex = 1;
            loginUsernameLabel.Text = "Username";
            // 
            // loginPasswordLabel
            // 
            loginPasswordLabel.AutoSize = true;
            loginPasswordLabel.Location = new Point(578, 393);
            loginPasswordLabel.Name = "loginPasswordLabel";
            loginPasswordLabel.Size = new Size(57, 15);
            loginPasswordLabel.TabIndex = 2;
            loginPasswordLabel.Text = "Password";
            // 
            // loginUsernameTextbox
            // 
            loginUsernameTextbox.Location = new Point(696, 298);
            loginUsernameTextbox.Name = "loginUsernameTextbox";
            loginUsernameTextbox.Size = new Size(267, 23);
            loginUsernameTextbox.TabIndex = 3;
            // 
            // loginPasswordTextbox
            // 
            loginPasswordTextbox.Location = new Point(696, 393);
            loginPasswordTextbox.Name = "loginPasswordTextbox";
            loginPasswordTextbox.PasswordChar = '*';
            loginPasswordTextbox.Size = new Size(267, 23);
            loginPasswordTextbox.TabIndex = 4;
            // 
            // LoginPageLoginButton
            // 
            LoginPageLoginButton.Location = new Point(582, 505);
            LoginPageLoginButton.Name = "LoginPageLoginButton";
            LoginPageLoginButton.Size = new Size(141, 35);
            LoginPageLoginButton.TabIndex = 5;
            LoginPageLoginButton.Text = "Login";
            LoginPageLoginButton.UseVisualStyleBackColor = true;
            LoginPageLoginButton.Click += LoginPageLoginButton_Click;
            // 
            // loginPageExitButton
            // 
            loginPageExitButton.Location = new Point(822, 505);
            loginPageExitButton.Name = "loginPageExitButton";
            loginPageExitButton.Size = new Size(141, 35);
            loginPageExitButton.TabIndex = 6;
            loginPageExitButton.Text = "Exit";
            loginPageExitButton.UseVisualStyleBackColor = true;
            loginPageExitButton.Click += LoginPageExitButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.Red;
            label1.Location = new Point(696, 437);
            label1.Name = "label1";
            label1.Size = new Size(0, 15);
            label1.TabIndex = 7;
            // 
            // LoginPageErrorLabel
            // 
            LoginPageErrorLabel.AutoSize = true;
            LoginPageErrorLabel.Location = new Point(696, 452);
            LoginPageErrorLabel.Name = "LoginPageErrorLabel";
            LoginPageErrorLabel.Size = new Size(0, 15);
            LoginPageErrorLabel.TabIndex = 8;
            LoginPageErrorLabel.Visible = false;
            // 
            // LoginPage
            // 
            ClientSize = new Size(2055, 877);
            Controls.Add(LoginPageErrorLabel);
            Controls.Add(label1);
            Controls.Add(loginPageExitButton);
            Controls.Add(LoginPageLoginButton);
            Controls.Add(loginPasswordTextbox);
            Controls.Add(loginUsernameTextbox);
            Controls.Add(loginPasswordLabel);
            Controls.Add(loginUsernameLabel);
            Name = "LoginPage";
            Load += LoginPage_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private void LoginPageLoginButton_Click(object sender, EventArgs e)
        {
            string username = loginUsernameTextbox.Text;
            string password = loginPasswordTextbox.Text;

            var userCredential = _loginManager.ValidateLogin(username, password);

            if (userCredential != null)
            {
                MessageBox.Show("Login successful.");
            }
            else
            {
                LoginPageErrorLabel.Visible = true;
                LoginPageErrorLabel.Text = "Login failed. Please check your username and password.";
            }
        }



        private void LoginPageExitButton_Click(object sender, EventArgs e)
        {

        }

        private void LoginPage_Load(object sender, EventArgs e)
        {

        }


