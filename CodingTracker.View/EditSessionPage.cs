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
using CodingTracker.View.IFormControllers;
using CodingTracker.Common.IDatabaseSessionReads;
using CodingTracker.Common.IApplicationLoggers;
using System.Diagnostics;

namespace CodingTracker.View
{
    public partial class EditSessionPage : Form
    {
        private readonly IApplicationControl _appControl;
        private readonly IFormSwitcher _formSwitcher;
        private readonly IFormController _formController;
        private readonly IDatabaseSessionRead _databaseSessionRead;
        private readonly IApplicationLogger _appLogger;
        public EditSessionPage(IApplicationControl appControl, IFormSwitcher formSwitcher, IDatabaseSessionRead databaseSessionRead, IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
            _appControl = appControl;
            _formSwitcher = formSwitcher;
            _databaseSessionRead = databaseSessionRead;
            InitializeComponent();
            InitializeDataGridView();
            LoadSessionsIntoDataGridView();
        }

        private void EditSessionPage_Load(object sender, EventArgs e)
        {
            LoadSessionsIntoDataGridView();
        }

        private void LoadSessionsIntoDataGridView()
        {
            var methodName = nameof(LoadSessionsIntoDataGridView);
            using (var activity = new Activity(methodName).Start())
            {
                _appLogger.Debug($"Starting {methodName}. TraceID: {activity.TraceId}");

                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    int numberOfSessions = 10;
                    var sessions = _databaseSessionRead.ViewRecentSession(numberOfSessions);

                    EditSessionPageDataGridView.Rows.Clear();

                    foreach (var session in sessions)
                    {
                        int rowIndex = EditSessionPageDataGridView.Rows.Add();
                        EditSessionPageDataGridView.Rows[rowIndex].Cells[0].Value = session.UserId;
                        EditSessionPageDataGridView.Rows[rowIndex].Cells[1].Value = session.StartTime?.ToString("g");
                        EditSessionPageDataGridView.Rows[rowIndex].Cells[2].Value = session.EndTime?.ToString("g");
                        EditSessionPageDataGridView.Rows[rowIndex].Cells[5].Value = session.DurationMinutes;
                    }

                    stopwatch.Stop();
                    _appLogger.Info($"Loaded sessions into DataGridView successfully. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to load sessions into DataGridView. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
            }
        }


        private void InitializeDataGridView()
        {

        }

        private void EditSessionExitControlBox_Click(object sender, EventArgs e)
        {
            _formController.CloseCurrentForm();
        }

        private void EditSessionPageEditSessionButton_Click(object sender, EventArgs e)
        {

        }
    }
}
