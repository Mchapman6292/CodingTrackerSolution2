using System;
using System.Windows.Forms;
using CodingTracker.Business.CodingSession;
using CodingTracker.Common.ICRUDs;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IInputValidators;
using CodingTracker.Common.IUtilityServices;
using CodingTracker.Common.UtilityServices;

namespace CodingTracker.View
{
    public partial class Form1 : Form
    {
        private readonly ICRUD _crud;
        private readonly IDatabaseManager _dbManager;
        private readonly IInputValidator _inputValidator;
        private readonly IUtilityService _utilityService;
        private readonly CodingSession _codingSession;


        public Form1(ICRUD crud, IDatabaseManager dbManager, IInputValidator inputValidator, IUtilityService utilityService, CodingSession codingSession)
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
                
                int goalHours = _utilityService.TryParseInt(GoalHoursTextBox.Text);
                _codingSession.SetCodingGoal(goalHours);
                MessageBox.Show("Coding goal set.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

    }
}
