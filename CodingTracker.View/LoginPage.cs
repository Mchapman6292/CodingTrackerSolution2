namespace CodingTracker.View
{
    public partial class LoginPage : Form
    {
        public LoginPage()
        {
            InitializeComponent();
            throw new NotImplementedException();
        }

        private void InitializeComponent()
        {
            loginUsernameLabel = new Label();
            loginPasswordLabel = new Label();
            loginUsernameTextbox = new TextBox();
            loginPasswordTextbox = new TextBox();
            mainPageLoginButton = new Button();
            mainPageExitButton = new Button();
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
            // mainPageLoginButton
            // 
            mainPageLoginButton.Location = new Point(582, 505);
            mainPageLoginButton.Name = "mainPageLoginButton";
            mainPageLoginButton.Size = new Size(141, 35);
            mainPageLoginButton.TabIndex = 5;
            mainPageLoginButton.Text = "Login";
            mainPageLoginButton.UseVisualStyleBackColor = true;
            mainPageLoginButton.Click += MainPageLoginButton;
            // 
            // mainPageExitButton
            // 
            mainPageExitButton.Location = new Point(822, 505);
            mainPageExitButton.Name = "mainPageExitButton";
            mainPageExitButton.Size = new Size(141, 35);
            mainPageExitButton.TabIndex = 6;
            mainPageExitButton.Text = "Exit";
            mainPageExitButton.UseVisualStyleBackColor = true;
            mainPageExitButton.Click += MainPageExitButton;
            // 
            // LoginPage
            // 
            ClientSize = new Size(2055, 877);
            Controls.Add(mainPageExitButton);
            Controls.Add(mainPageLoginButton);
            Controls.Add(loginPasswordTextbox);
            Controls.Add(loginUsernameTextbox);
            Controls.Add(loginPasswordLabel);
            Controls.Add(loginUsernameLabel);
            Name = "LoginPage";
            ResumeLayout(false);
            PerformLayout();
        }



        private void MainPageLoginButton(object sender, EventArgs e)
        {

        }

        private void MainPageExitButton(object sender, EventArgs e)
        {

        }
    }
}
