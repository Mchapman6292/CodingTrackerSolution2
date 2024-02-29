﻿using System;
using System.Windows.Forms;
using CodingTracker.Business.CodingSession;
using CodingTracker.Logging.ICRUDs;
using CodingTracker.Logging.IDatabaseManagers;
using CodingTracker.Logging.IInputValidators;
using CodingTracker.Logging.IUtilityServices;
using CodingTracker.Logging.UtilityServices;

namespace CodingTracker.View
{
    public partial class LoginPage : Form
    {
        private readonly ICRUD _crud;
        private readonly IDatabaseManager _dbManager;
        private readonly IInputValidator _inputValidator;
        private readonly IUtilityService _utilityService;
        private readonly CodingSession _codingSession;
        private Button startSessionButton;
        private Button endSessionButton;
        private Button viewSessionsButton;
        private Button setGoalButton;
        private TextBox goalHoursTextBox;
        private DataGridView sessionsDataGridView;



        public LoginPage(ICRUD crud, IDatabaseManager dbManager, IInputValidator inputValidator, IUtilityService utilityService, CodingSession codingSession)
        {
            InitializeComponent();
            _crud = crud;
            _dbManager = dbManager;
            _inputValidator = inputValidator;
            _utilityService = utilityService;
            _codingSession = codingSession;

            // Initialize GUI components here
            //add buttons, text boxes, labels, and data grid
        }




        private void InitializeFormComponents()
        {
            
            startSessionButton = new Button { Text = "Start Session" };
            endSessionButton = new Button { Text = "End Session" };
            viewSessionsButton = new Button { Text = "View Sessions" };
            setGoalButton = new Button { Text = "Set Goal" };
            goalHoursTextBox = new TextBox();
            sessionsDataGridView = new DataGridView();


            startSessionButton.Location = new Point(10, 10);
            endSessionButton.Location = new Point(110, 10);
            viewSessionsButton.Location = new Point(210, 10);
            setGoalButton.Location = new Point(310, 10);
            goalHoursTextBox.Location = new Point(410, 10);
            sessionsDataGridView.Location = new Point(10, 50);


            startSessionButton.Click += StartSessionButton_Click;
            endSessionButton.Click += EndSessionButton_Click;
            viewSessionsButton.Click += ViewSessionsButton_Click;
            setGoalButton.Click += SetGoalButton_Click;


            Controls.Add(startSessionButton);
            Controls.Add(endSessionButton);
            Controls.Add(viewSessionsButton);
            Controls.Add(setGoalButton);
            Controls.Add(goalHoursTextBox);
            Controls.Add(sessionsDataGridView);
        }

        private void StartSessionButton_Click(object sender, EventArgs e)
        {
            try
            {
                _codingSession.StartSession();
                MessageBox.Show("Session started.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void EndSessionButton_Click(object sender, EventArgs e)
        {
            try
            {
                _codingSession.EndSession();
                MessageBox.Show("Session ended.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ViewSessionsButton_Click(object sender, EventArgs e)
        {
            try
            {


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void SetGoalButton_Click(object sender, EventArgs e)
        {
            try
            {
                
                int goalHours = _utilityService.TryParseInt(goalHoursTextBox.Text);
                _codingSession.SetCodingGoal(goalHours);
                MessageBox.Show("Coding goal set.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private Label loginUsernameLabel;
        private Label loginPasswordLabel;
        private TextBox loginUsernameTextbox;
        private TextBox loginPasswordTextbox;
        private Button mainPageLoginButton;
        private Button mainPageExitButton;
    }
}
