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
using CodingTracker.View.FormSwitchers;
using CodingTracker.View.FormControllers;
using CodingTracker.Common.Interfaces.ICodingSessionRepository;
using CodingTracker.Common.IApplicationLoggers;
using System.Diagnostics;
using Guna.UI2.WinForms;

namespace CodingTracker.View
{
    public partial class EditSessionPage : Form
    {
        private readonly IApplicationControl _appControl;
        private readonly IFormSwitcher _formSwitcher;
        private readonly IFormController _formController;
        private readonly IApplicationLogger _appLogger;
        private readonly ICodingSessionRepository _codingSessionRepository;
        private List<int> deletionSessionIds = new List<int>();
        private bool isEditSessionOn = false;

        public EditSessionPage(IApplicationControl appControl, IFormSwitcher formSwitcher, IApplicationLogger appLogger, ICodingSessionRepository codingSessionRepository)
        {
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
            using (var activity = new Activity(nameof(LoadSessionsIntoDataGridView)).Start())
            {
                _appLogger.Debug($"Starting {nameof(LoadSessionsIntoDataGridView)}. TraceID: {activity.TraceId}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    int numberOfSessions = 20;
                    var sessions = await _codingSessionRepository.GetRecentSessions(activity, numberOfSessions);

                    EditSessionPageDataGridView.Rows.Clear();

                    foreach (var session in sessions)
                    {
                        int rowIndex = EditSessionPageDataGridView.Rows.Add();
                        EditSessionPageDataGridView.Rows[rowIndex].Cells[0].Value = session.SessionId;
                        EditSessionPageDataGridView.Rows[rowIndex].Cells[1].Value = session.SessionId;
                        EditSessionPageDataGridView.Rows[rowIndex].Cells[2].Value = session.DurationHHMM;
                        EditSessionPageDataGridView.Rows[rowIndex].Cells[3].Value = session.StartTime?.ToString("g");
                        EditSessionPageDataGridView.Rows[rowIndex].Cells[4].Value = session.EndTime?.ToString("g");

                        _appLogger.Debug($"Added session to DataGridView: SessionID={session.SessionId}, StartTime={session.StartTime}, EndTime={session.EndTime}, DurationSeconds={session.DurationSeconds}. RowIndex={rowIndex}. TraceID={activity.TraceId}");
                    }

                    stopwatch.Stop();
                    _appLogger.Info($"Loaded sessions into DataGridView successfully. Total sessions loaded: {sessions.Count()}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to load sessions into DataGridView. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
            }
        }

        private void EditModeDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            using (var activity = new Activity(nameof(EditModeDataGridView_CellClick)).Start())
            {
                _appLogger.Debug($"Starting {nameof(EditModeDataGridView_CellClick)}: TraceID: {activity.TraceId}");
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    if (isEditSessionOn && e.RowIndex >= 0)
                    {
                        DataGridViewRow row = EditSessionPageDataGridView.Rows[e.RowIndex];
                        int sessionId = Convert.ToInt32(row.Cells["SessionId"].Value);

                        bool highlight = !deletionSessionIds.Contains(sessionId);
                        if (highlight)
                        {
                            deletionSessionIds.Add(sessionId);
                        }
                        else
                        {
                            deletionSessionIds.Remove(sessionId);
                        }
                        HighlightRow(row, highlight);
                        _appLogger.Info($"Row clicked: SessionID={sessionId}, Highlighted={highlight}, TraceID={activity.TraceId}");
                    }
                    stopwatch.Stop();
                    _appLogger.Info($"{nameof(EditModeDataGridView_CellClick)} completed successfully. Execution Time: {stopwatch.ElapsedMilliseconds}ms, TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed during {nameof(EditModeDataGridView_CellClick)}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms, TraceID: {activity.TraceId}");
                }
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

        private void ClearDeletionSessionIdsList()
        {
            using(var activity = new Activity(nameof(ClearDeletionSessionIdsList)).Start())
            {
                _appLogger.Info($"Starting {nameof(ClearDeletionSessionIdsList)}. TraceID: {activity.TraceId}.");
                Stopwatch stopwatch = Stopwatch.StartNew();

                deletionSessionIds.Clear();
                stopwatch.Stop();

                _appLogger.Info($"{nameof(ClearDeletionSessionIdsList)} complete, elapsed time : {stopwatch.ElapsedMilliseconds}, TraceID: {activity.TraceId}");
            }
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
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var activity = new Activity(nameof(ChangeButtonColorIfEditSession)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ChangeButtonColorIfEditSession)}, TraceID: {activity.TraceId}.");

                if (!isEditSessionOn) 
                {
                    EditSessionButton.ForeColor = Color.White;
                }
                else
                {
                    EditSessionButton.ForeColor = Color.FromArgb(193, 20, 137); // Default dark pink
                }
                stopwatch.Stop();
                _appLogger.Info($"{nameof(EditSessionButton_Click)}: completed TraceID: {activity.TraceId}. Elapsed time: {stopwatch.ElapsedMilliseconds}.");
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
            using (var activity = new Activity(nameof(DeleteSessionButton_Click)).Start())
            {
                if (!isEditSessionOn)
                {
                    _appLogger.Error($"Error for {nameof(DeleteSessionButton_Click)}. isEditSessionOn is set to false, session editing must be enabled to delete sessions. TraceID: {activity.TraceId}");
                    return;
                }

                if (deletionSessionIds.Count == 0)
                {
                    _appLogger.Error($"Error for {nameof(DeleteSessionButton_Click)}. deletionSessionIds list is empty. TraceID: {activity.TraceId}");
                    return;
                }

                try
                {
                    DeleteSessionButton.Enabled = false;

                    _appLogger.Info($"Starting deletion of {deletionSessionIds.Count} sessions. TraceID: {activity.TraceId}");

                    var (success, failedIds) = await _codingSessionRepository.DeleteSessionsById(activity, deletionSessionIds);

                    if (success)
                    {
                        _appLogger.Info($"Successfully deleted {deletionSessionIds.Count - failedIds.Count} out of {deletionSessionIds.Count} sessions. TraceID: {activity.TraceId}");
                    }
                    else
                    {
                        _appLogger.Warning($"Failed to delete some sessions. {failedIds.Count} deletions failed. TraceID: {activity.TraceId}");
                    }
                    await LoadSessionsIntoDataGridView();
                    deletionSessionIds.Clear();
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"Error in {nameof(DeleteSessionButton_Click)}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
                finally
                {
                    DeleteSessionButton.Enabled = true;
                }
            }
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
