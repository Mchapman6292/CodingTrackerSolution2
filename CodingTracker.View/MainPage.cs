using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.View.FormControllers;
using CodingTracker.View.IFormControllers;

namespace CodingTracker.View
{
    public partial class MainPage : Form
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IFormController _formController;

        public MainPage(IApplicationLogger applogger, IFormController formController)
        {
            InitializeComponent();
            _appLogger = applogger;
            _formController = formController;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void MainPageCodingSessionButton_Click(object sender, EventArgs e)
        {
            using (var activity = new Activity(nameof(MainPageCodingSessionButton_Click)))
            {
                _appLogger.Debug($"Starting {MainPageCodingSessionButton_Click}. TraceID: {activity.TraceId}.");
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    _formController.ShowCodingSessionPage();
                    stopwatch.Stop();
                    _appLogger.Info($"Successfully displayed Coding Session Page. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to display Coding Session Page. Error: {ex.Message}. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);


                    MessageBox.Show("Unable to open the Coding Session Page. Please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void MainPageEditSessionsButton_Click(object sender, EventArgs e)
        {
            using (var activity = new Activity(nameof(MainPageEditSessionsButton_Click)).Start())
            {
                _appLogger.Debug($"Starting {nameof(MainPageEditSessionsButton_Click)}. TraceID: {activity.TraceId}");
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    _formController.ShowEditSessionPage();
                    stopwatch.Stop();
                    _appLogger.Info($"Successfully displayed Edit Session Page. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to display Edit Session Page. Error: {ex.Message}. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    MessageBox.Show("Unable to open the Edit Session Page. Please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MainPageViewSessionsButton_Click(object sender, EventArgs e)
        {
            using (var activity = new Activity(nameof(MainPageViewSessionsButton_Click)).Start())
            {
                _appLogger.Debug($"Starting {nameof(MainPageViewSessionsButton_Click)}. TraceID: {activity.TraceId}");
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    _formController.ShowViewSessionPage();
                    stopwatch.Stop();
                    _appLogger.Info($"Successfully displayed View Session Page. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to display View Session Page. Error: {ex.Message}. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    MessageBox.Show("Unable to open the View Session Page. Please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MainPageSettingsButton_Click(object sender, EventArgs e)
        {
            using (var activity = new Activity(nameof(MainPageSettingsButton_Click)).Start())
            {
                _appLogger.Debug($"Starting {nameof(MainPageSettingsButton_Click)}. TraceID: {activity.TraceId}");
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    _formController.ShowSettingsPage();
                    stopwatch.Stop();
                    _appLogger.Info($"Successfully displayed Settings Page. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Failed to display Settings Page. Error: {ex.Message}. Duration: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    MessageBox.Show("Unable to open the Settings Page. Please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
