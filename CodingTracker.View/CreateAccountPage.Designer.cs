namespace CodingTracker.View
{
    partial class CreateAccountPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            CreateAccountPageUsernameTextbox = new Guna.UI2.WinForms.Guna2TextBox();
            CreateAccountPasswordTextbox = new Guna.UI2.WinForms.Guna2TextBox();
            SuspendLayout();
            // 
            // CreateAccountPageUsernameTextbox
            // 
            CreateAccountPageUsernameTextbox.AutoRoundedCorners = true;
            CreateAccountPageUsernameTextbox.BorderColor = Color.FromArgb(234, 153, 149);
            CreateAccountPageUsernameTextbox.BorderRadius = 17;
            CreateAccountPageUsernameTextbox.CustomizableEdges = customizableEdges1;
            CreateAccountPageUsernameTextbox.DefaultText = "";
            CreateAccountPageUsernameTextbox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            CreateAccountPageUsernameTextbox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            CreateAccountPageUsernameTextbox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            CreateAccountPageUsernameTextbox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            CreateAccountPageUsernameTextbox.FillColor = Color.FromArgb(35, 34, 50);
            CreateAccountPageUsernameTextbox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            CreateAccountPageUsernameTextbox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CreateAccountPageUsernameTextbox.ForeColor = Color.FromArgb(35, 34, 50);
            CreateAccountPageUsernameTextbox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            CreateAccountPageUsernameTextbox.Location = new Point(450, 208);
            CreateAccountPageUsernameTextbox.Name = "CreateAccountPageUsernameTextbox";
            CreateAccountPageUsernameTextbox.PasswordChar = '\0';
            CreateAccountPageUsernameTextbox.PlaceholderForeColor = Color.Azure;
            CreateAccountPageUsernameTextbox.PlaceholderText = "Username";
            CreateAccountPageUsernameTextbox.SelectedText = "";
            CreateAccountPageUsernameTextbox.ShadowDecoration.CustomizableEdges = customizableEdges2;
            CreateAccountPageUsernameTextbox.Size = new Size(200, 36);
            CreateAccountPageUsernameTextbox.TabIndex = 16;
            // 
            // CreateAccountPasswordTextbox
            // 
            CreateAccountPasswordTextbox.AutoRoundedCorners = true;
            CreateAccountPasswordTextbox.BorderColor = Color.FromArgb(234, 153, 149);
            CreateAccountPasswordTextbox.BorderRadius = 17;
            CreateAccountPasswordTextbox.CustomizableEdges = customizableEdges3;
            CreateAccountPasswordTextbox.DefaultText = "";
            CreateAccountPasswordTextbox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            CreateAccountPasswordTextbox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            CreateAccountPasswordTextbox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            CreateAccountPasswordTextbox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            CreateAccountPasswordTextbox.FillColor = Color.FromArgb(35, 34, 50);
            CreateAccountPasswordTextbox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            CreateAccountPasswordTextbox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CreateAccountPasswordTextbox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            CreateAccountPasswordTextbox.Location = new Point(450, 312);
            CreateAccountPasswordTextbox.Name = "CreateAccountPasswordTextbox";
            CreateAccountPasswordTextbox.PasswordChar = '\0';
            CreateAccountPasswordTextbox.PlaceholderForeColor = Color.Azure;
            CreateAccountPasswordTextbox.PlaceholderText = "Password";
            CreateAccountPasswordTextbox.SelectedText = "";
            CreateAccountPasswordTextbox.ShadowDecoration.CustomizableEdges = customizableEdges4;
            CreateAccountPasswordTextbox.Size = new Size(200, 36);
            CreateAccountPasswordTextbox.TabIndex = 17;
            // 
            // CreateAccountPage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(35, 34, 50);
            ClientSize = new Size(1284, 681);
            Controls.Add(CreateAccountPasswordTextbox);
            Controls.Add(CreateAccountPageUsernameTextbox);
            Name = "CreateAccountPage";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2TextBox CreateAccountPageUsernameTextbox;
        private Guna.UI2.WinForms.Guna2TextBox CreateAccountPasswordTextbox;
    }
}