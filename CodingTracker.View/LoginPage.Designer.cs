using System;
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
            


            startSessionButton.Location = new Point(10, 10);
            endSessionButton.Location = new Point(110, 10);
            viewSessionsButton.Location = new Point(210, 10);
            setGoalButton.Location = new Point(310, 10);
            goalHoursTextBox.Location = new Point(410, 10);
            sessionsDataGridView.Location = new Point(10, 50);




            Controls.Add(startSessionButton);
            Controls.Add(endSessionButton);
            Controls.Add(viewSessionsButton);
            Controls.Add(setGoalButton);
            Controls.Add(goalHoursTextBox);
            Controls.Add(sessionsDataGridView);
        }


        private Label loginUsernameLabel;
        private Label loginPasswordLabel;
        private TextBox loginUsernameTextbox;
        private TextBox loginPasswordTextbox;
        private Button LoginPageLoginButton;
        private Button loginPageExitButton;
        private Label label1;
        private Label LoginPageErrorLabel;
    }
}
