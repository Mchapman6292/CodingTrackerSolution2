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
using CodingTracker.Common.DataInterfaces.ICodingSessionRepositories;
using CodingTracker.Common.IApplicationLoggers;
using System.Diagnostics;
using CodingTracker.Business.CodingSessionService.EditSessionPageContextManagers;
using Guna.UI2.WinForms;
using CodingTracker.Data.Repositories.CodingSessionRepositories;
using CodingTracker.Common.Entities.CodingSessionEntities;
using CodingTracker.View.FormService;

namespace CodingTracker.View
{
    public partial class EditSessionPage : Form
    {
        private readonly EditSessionPageContextManager EditPageContextManager;


        private readonly IApplicationControl _appControl;
        private readonly IFormSwitcher _formSwitcher;
        private readonly IFormController _formController;
        private readonly IApplicationLogger _appLogger;
        private readonly CodingSessionRepository _codingSessionRepository;
        private bool isEditSessionOn = false;

        public EditSessionPage(IApplicationControl appControl, IFormSwitcher formSwitcher, IApplicationLogger appLogger, CodingSessionRepository codingSessionRepository, EditSessionPageContextManager editContextManager)
        {
            EditPageContextManager = editContextManager;
            _appLogger = appLogger;
            _appControl = appControl;
            _formSwitcher = formSwitcher;
            _codingSessionRepository = codingSessionRepository;
            InitializeComponent();
            LoadSessionsIntoDataGridView();
        }

        private void EditSessionPage_Load(object sender, EventArgs e)
        {
            LoadSessionsIntoDataGridView();
        }


        private async Task LoadSessionsIntoDataGridView()
        {
            int numberOfSessions = 20;

            List<CodingSessionEntity> sessions = await _codingSessionRepository.GetRecentSessionsAsync(numberOfSessions);

            EditSessionPageDataGridView.Rows.Clear();

            foreach (var session in sessions)
            {
                int rowIndex = EditSessionPageDataGridView.Rows.Add();
                if(rowIndex < 0)
                {
                    _appLogger.Error($"Failed to add row for SessionID {session.SessionId}. Invalid row index returned.");
                    continue;
                }

                EditSessionPageDataGridView.Rows[rowIndex].Cells[0].Value = session.SessionId;
                EditSessionPageDataGridView.Rows[rowIndex].Cells[1].Value = session.SessionId;
                EditSessionPageDataGridView.Rows[rowIndex].Cells[2].Value = session.DurationHHMM;
                EditSessionPageDataGridView.Rows[rowIndex].Cells[3].Value = session.StartDate?.ToString("g");
                EditSessionPageDataGridView.Rows[rowIndex].Cells[4].Value = session.EndDate?.ToString("g");

            }
        }
         
        

        private void EditModeDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (isEditSessionOn && e.RowIndex >= 0)
            {
                DataGridViewRow row = EditSessionPageDataGridView.Rows[e.RowIndex];
                int sessionId = Convert.ToInt32(row.Cells["SessionId"].Value);

                bool highlight = EditPageContextManager.CheckForSessionId(sessionId);
                if (highlight)
                {
                    EditPageContextManager.AddSessionIdForDeletion(sessionId);
                }
                else
                {
                    EditPageContextManager.RemoveSessionIdForDeletion(sessionId);
                }
                HighlightRow(row, highlight);
            }
        }
            
        


        private void HighlightRow(DataGridViewRow row, bool highlight)
        {
            using (var activity = new Activity(nameof(HighlightRow)).Start())
            {
                try
                {
                    if (highlight)
                    {
                        row.DefaultCellStyle.BackColor = Color.DarkOrange;
                        row.DefaultCellStyle.ForeColor = Color.White;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = EditSessionPageDataGridView.DefaultCellStyle.BackColor;
                        row.DefaultCellStyle.ForeColor = EditSessionPageDataGridView.DefaultCellStyle.ForeColor;
                    }
                    _appLogger.Debug($"{nameof(HighlightRow)} executed: RowIndex={row.Index}, Highlighted={highlight}, TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"Error in {nameof(HighlightRow)}: {ex.Message}, TraceID: {activity.TraceId}");
                }
            }
        }

        private void EditSessionExitControlBox_Click(object sender, EventArgs e)
        {
            _formController.CloseCurrentForm();
        }

        private void EditSessionButton_Click(object sender, EventArgs e)
        {
            ToggleEditMode();
            SetDataGridViewEditMode();
            DeleteSessionButton.Visible = true;
        }



        private void ToggleEditMode()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(ToggleEditMode)).Start())
            {
                if (!isEditSessionOn)
                {
                    isEditSessionOn = true;
                    EditSessionPageDataGridView.BackgroundColor = Color.Yellow;
                    _appLogger.Info("isEditSessionOn bool updated to true");
                }
                else
                {
                    isEditSessionOn = false;
                    EditSessionPageDataGridView.BackgroundColor = Color.FromArgb(35, 34, 50);
                    _appLogger.Info("isEditSessionOn bool updated to false");
                }
                stopwatch.Stop();
                _appLogger.Info($" {nameof(ToggleEditMode)} completed, TraceID: {activity.TraceId}, elapsed time: {stopwatch.ElapsedMilliseconds}."); 
            }
        }

        private void ChangeButtonColorIfEditSession()
        {
            if (!isEditSessionOn) 
            {
                EditSessionButton.ForeColor = Color.White;
            }
            else
            {
                EditSessionButton.ForeColor = Color.FromArgb(193, 20, 137); // Default dark pink
            }
        }

        private void SetDataGridViewEditMode()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(SetDataGridViewEditMode)).Start())
            {
                _appLogger.Debug($"Starting {nameof(SetDataGridViewEditMode)}. TraceID: {activity.TraceId}.");

                if (!isEditSessionOn)
                {
                    EditSessionButton.ForeColor = Color.White;
                }
                else 
                {
                    EditSessionPageDataGridView.DefaultCellStyle.SelectionForeColor = Color.FromArgb(255, 140, 0);
                }
            }
        }

        private void UpdateColorsForSelectedSessionsInEditMode()
        {
            throw new NotImplementedException();
        }



        private async void DeleteSessionButton_Click(object sender, EventArgs e)
        {
            if(!isEditSessionOn)
            {
                _appLogger.Error($"Error for {nameof(DeleteSessionButton_Click)}. isEditSessionOn is set to false, session editing must be enabled to delete sessions.");
            }

            DeleteSessionButton.Enabled = false; // Disabled during deletion to prevent multiple clicks etc.

            IReadOnlyCollection<int> deletedSessionIds = EditPageContextManager.GetSessionIdsForDeletion();


        }








        private void EditSessionPageBackButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            _formSwitcher.SwitchToMainPage();
        }

        private void CodingSessionPageHomeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            _formSwitcher.SwitchToMainPage();
        }

    }
}
