using Guna.UI2.WinForms.Suite;

namespace CodingTracker.View
{
    partial class CodingSessionPage
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
            CustomizableEdges customizableEdges19 = new CustomizableEdges();
            CustomizableEdges customizableEdges20 = new CustomizableEdges();
            CustomizableEdges customizableEdges16 = new CustomizableEdges();
            CustomizableEdges customizableEdges17 = new CustomizableEdges();
            CustomizableEdges customizableEdges18 = new CustomizableEdges();
            CustomizableEdges customizableEdges21 = new CustomizableEdges();
            CustomizableEdges customizableEdges22 = new CustomizableEdges();
            CustomizableEdges customizableEdges27 = new CustomizableEdges();
            CustomizableEdges customizableEdges28 = new CustomizableEdges();
            CustomizableEdges customizableEdges23 = new CustomizableEdges();
            CustomizableEdges customizableEdges24 = new CustomizableEdges();
            CustomizableEdges customizableEdges25 = new CustomizableEdges();
            CustomizableEdges customizableEdges26 = new CustomizableEdges();
            CustomizableEdges customizableEdges29 = new CustomizableEdges();
            CustomizableEdges customizableEdges30 = new CustomizableEdges();
            CodingSessionPageCentrePanel = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            CodingSessionPageSetSessionGoalButton = new Guna.UI2.WinForms.Guna2Button();
            CodingSessionPageStartSessionButton = new Guna.UI2.WinForms.Guna2CircleButton();
            CodingSessionPageMinimiseButton = new Guna.UI2.WinForms.Guna2Button();
            CodingSesisonPageTopPanel = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            guna2CustomGradientPanel1 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            CodingSessionPageExitButton = new Guna.UI2.WinForms.Guna2Button();
            CodingSessionPageSessionGoalInputPanel = new Guna.UI2.WinForms.Guna2ShadowPanel();
            CodingSessionPageSessionGoalTextBox = new Guna.UI2.WinForms.Guna2TextBox();
            CodingSessionPageCentrePanel.SuspendLayout();
            CodingSesisonPageTopPanel.SuspendLayout();
            CodingSessionPageSessionGoalInputPanel.SuspendLayout();
            SuspendLayout();
            // 
            // CodingSessionPageCentrePanel
            // 
            CodingSessionPageCentrePanel.Controls.Add(CodingSessionPageSessionGoalInputPanel);
            CodingSessionPageCentrePanel.Controls.Add(CodingSessionPageSetSessionGoalButton);
            CodingSessionPageCentrePanel.Controls.Add(CodingSessionPageStartSessionButton);
            CodingSessionPageCentrePanel.CustomizableEdges = customizableEdges19;
            CodingSessionPageCentrePanel.Location = new Point(4, 136);
            CodingSessionPageCentrePanel.Name = "CodingSessionPageCentrePanel";
            CodingSessionPageCentrePanel.ShadowDecoration.CustomizableEdges = customizableEdges20;
            CodingSessionPageCentrePanel.Size = new Size(1300, 300);
            CodingSessionPageCentrePanel.TabIndex = 0;
            // 
            // CodingSessionPageSetSessionGoalButton
            // 
            CodingSessionPageSetSessionGoalButton.CustomizableEdges = customizableEdges16;
            CodingSessionPageSetSessionGoalButton.DisabledState.BorderColor = Color.DarkGray;
            CodingSessionPageSetSessionGoalButton.DisabledState.CustomBorderColor = Color.DarkGray;
            CodingSessionPageSetSessionGoalButton.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            CodingSessionPageSetSessionGoalButton.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            CodingSessionPageSetSessionGoalButton.Font = new Font("Segoe UI", 9F);
            CodingSessionPageSetSessionGoalButton.ForeColor = Color.White;
            CodingSessionPageSetSessionGoalButton.Location = new Point(734, 168);
            CodingSessionPageSetSessionGoalButton.Name = "CodingSessionPageSetSessionGoalButton";
            CodingSessionPageSetSessionGoalButton.ShadowDecoration.CustomizableEdges = customizableEdges17;
            CodingSessionPageSetSessionGoalButton.Size = new Size(180, 45);
            CodingSessionPageSetSessionGoalButton.TabIndex = 2;
            CodingSessionPageSetSessionGoalButton.Text = "Set session goal";
            CodingSessionPageSetSessionGoalButton.Click += CodingSessionPageSetSessionGoalButton_Click;
            // 
            // CodingSessionPageStartSessionButton
            // 
            CodingSessionPageStartSessionButton.DisabledState.BorderColor = Color.DarkGray;
            CodingSessionPageStartSessionButton.DisabledState.CustomBorderColor = Color.DarkGray;
            CodingSessionPageStartSessionButton.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            CodingSessionPageStartSessionButton.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            CodingSessionPageStartSessionButton.Font = new Font("Segoe UI", 9F);
            CodingSessionPageStartSessionButton.ForeColor = Color.White;
            CodingSessionPageStartSessionButton.Location = new Point(534, 79);
            CodingSessionPageStartSessionButton.Name = "CodingSessionPageStartSessionButton";
            CodingSessionPageStartSessionButton.ShadowDecoration.CustomizableEdges = customizableEdges18;
            CodingSessionPageStartSessionButton.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            CodingSessionPageStartSessionButton.Size = new Size(148, 148);
            CodingSessionPageStartSessionButton.TabIndex = 1;
            CodingSessionPageStartSessionButton.Text = "StartCountDownTimer Coding Session";
            CodingSessionPageStartSessionButton.Click += CodingSessionPageStartSessionButton_Click;
            // 
            // CodingSessionPageMinimiseButton
            // 
            CodingSessionPageMinimiseButton.CustomizableEdges = customizableEdges21;
            CodingSessionPageMinimiseButton.DisabledState.BorderColor = Color.DarkGray;
            CodingSessionPageMinimiseButton.DisabledState.CustomBorderColor = Color.DarkGray;
            CodingSessionPageMinimiseButton.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            CodingSessionPageMinimiseButton.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            CodingSessionPageMinimiseButton.Font = new Font("Segoe UI", 9F);
            CodingSessionPageMinimiseButton.ForeColor = Color.White;
            CodingSessionPageMinimiseButton.Location = new Point(1143, -2);
            CodingSessionPageMinimiseButton.Name = "CodingSessionPageMinimiseButton";
            CodingSessionPageMinimiseButton.ShadowDecoration.CustomizableEdges = customizableEdges22;
            CodingSessionPageMinimiseButton.Size = new Size(78, 48);
            CodingSessionPageMinimiseButton.TabIndex = 3;
            CodingSessionPageMinimiseButton.Text = "Minimise";
            CodingSessionPageMinimiseButton.Click += CodingSessionPageMinimiseButton_Click;
            // 
            // CodingSesisonPageTopPanel
            // 
            CodingSesisonPageTopPanel.Controls.Add(CodingSessionPageMinimiseButton);
            CodingSesisonPageTopPanel.Controls.Add(guna2CustomGradientPanel1);
            CodingSesisonPageTopPanel.Controls.Add(CodingSessionPageExitButton);
            CodingSesisonPageTopPanel.CustomizableEdges = customizableEdges27;
            CodingSesisonPageTopPanel.Dock = DockStyle.Top;
            CodingSesisonPageTopPanel.Location = new Point(0, 0);
            CodingSesisonPageTopPanel.Name = "CodingSesisonPageTopPanel";
            CodingSesisonPageTopPanel.ShadowDecoration.CustomizableEdges = customizableEdges28;
            CodingSesisonPageTopPanel.Size = new Size(1300, 46);
            CodingSesisonPageTopPanel.TabIndex = 1;
            // 
            // guna2CustomGradientPanel1
            // 
            guna2CustomGradientPanel1.CustomizableEdges = customizableEdges23;
            guna2CustomGradientPanel1.Location = new Point(0, -83);
            guna2CustomGradientPanel1.Name = "guna2CustomGradientPanel1";
            guna2CustomGradientPanel1.ShadowDecoration.CustomizableEdges = customizableEdges24;
            guna2CustomGradientPanel1.Size = new Size(1300, 72);
            guna2CustomGradientPanel1.TabIndex = 4;
            // 
            // CodingSessionPageExitButton
            // 
            CodingSessionPageExitButton.CustomizableEdges = customizableEdges25;
            CodingSessionPageExitButton.DisabledState.BorderColor = Color.DarkGray;
            CodingSessionPageExitButton.DisabledState.CustomBorderColor = Color.DarkGray;
            CodingSessionPageExitButton.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            CodingSessionPageExitButton.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            CodingSessionPageExitButton.Font = new Font("Segoe UI", 9F);
            CodingSessionPageExitButton.ForeColor = Color.White;
            CodingSessionPageExitButton.Location = new Point(1227, 0);
            CodingSessionPageExitButton.Name = "CodingSessionPageExitButton";
            CodingSessionPageExitButton.ShadowDecoration.CustomizableEdges = customizableEdges26;
            CodingSessionPageExitButton.Size = new Size(73, 45);
            CodingSessionPageExitButton.TabIndex = 5;
            CodingSessionPageExitButton.Text = "Exit";
            CodingSessionPageExitButton.Click += guna2Button1_Click;
            // 
            // CodingSessionPageSessionGoalInputPanel
            // 
            CodingSessionPageSessionGoalInputPanel.BackColor = Color.Transparent;
            CodingSessionPageSessionGoalInputPanel.Controls.Add(CodingSessionPageSessionGoalTextBox);
            CodingSessionPageSessionGoalInputPanel.FillColor = Color.White;
            CodingSessionPageSessionGoalInputPanel.Location = new Point(956, 163);
            CodingSessionPageSessionGoalInputPanel.Name = "CodingSessionPageSessionGoalInputPanel";
            CodingSessionPageSessionGoalInputPanel.ShadowColor = Color.Black;
            CodingSessionPageSessionGoalInputPanel.Size = new Size(200, 50);
            CodingSessionPageSessionGoalInputPanel.TabIndex = 3;
            CodingSessionPageSessionGoalInputPanel.Visible = false;
            // 
            // CodingSessionPageSessionGoalTextBox
            // 
            CodingSessionPageSessionGoalTextBox.CustomizableEdges = customizableEdges29;
            CodingSessionPageSessionGoalTextBox.DefaultText = "";
            CodingSessionPageSessionGoalTextBox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            CodingSessionPageSessionGoalTextBox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            CodingSessionPageSessionGoalTextBox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            CodingSessionPageSessionGoalTextBox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            CodingSessionPageSessionGoalTextBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            CodingSessionPageSessionGoalTextBox.Font = new Font("Segoe UI", 9F);
            CodingSessionPageSessionGoalTextBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            CodingSessionPageSessionGoalTextBox.Location = new Point(20, 5);
            CodingSessionPageSessionGoalTextBox.Name = "CodingSessionPageSessionGoalTextBox";
            CodingSessionPageSessionGoalTextBox.PasswordChar = '\0';
            CodingSessionPageSessionGoalTextBox.PlaceholderText = "";
            CodingSessionPageSessionGoalTextBox.SelectedText = "";
            CodingSessionPageSessionGoalTextBox.ShadowDecoration.CustomizableEdges = customizableEdges30;
            CodingSessionPageSessionGoalTextBox.Size = new Size(152, 42);
            CodingSessionPageSessionGoalTextBox.TabIndex = 0;
            // 
            // CodingSessionPage
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1300, 720);
            Controls.Add(CodingSesisonPageTopPanel);
            Controls.Add(CodingSessionPageCentrePanel);
            Font = new Font("Century Gothic", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            Name = "CodingSessionPage";
            Text = "CodingSessionPage";
            WindowState = FormWindowState.Maximized;
            CodingSessionPageCentrePanel.ResumeLayout(false);
            CodingSesisonPageTopPanel.ResumeLayout(false);
            CodingSessionPageSessionGoalInputPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2CustomGradientPanel CodingSessionPageCentrePanel;
        private Guna.UI2.WinForms.Guna2CircleButton CodingSessionPageStartSessionButton;
        private Guna.UI2.WinForms.Guna2Button CodingSessionPageMinimiseButton;
        private Guna.UI2.WinForms.Guna2Button CodingSessionPageSetSessionGoalButton;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel CodingSesisonPageTopPanel;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel1;
        private Guna.UI2.WinForms.Guna2Button CodingSessionPageExitButton;
        private CustomizableEdges customizableEdges14;
        private CustomizableEdges customizableEdges15;
        private Guna.UI2.WinForms.Guna2ShadowPanel CodingSessionPageSessionGoalInputPanel;
        private Guna.UI2.WinForms.Guna2TextBox CodingSessionPageSessionGoalTextBox;
    }
}