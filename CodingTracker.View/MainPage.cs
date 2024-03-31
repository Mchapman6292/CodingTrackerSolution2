﻿using System.Data;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.View.IFormControllers;
using CodingTracker.Common.IPanelColorControls;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.IDatabaseSessionReads;
using CodingTracker.Common.ICodingSessions;
using CodingTracker.View.IFormFactories;



namespace CodingTracker.View
{
    public partial class MainPage : Form
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IFormController _formController;
        private readonly IPanelColorControl _panelColorControl;
        private readonly IErrorHandler _errorHandler;
        private readonly IPanelColorControl _colorControl;
        private readonly IDatabaseSessionRead _databaseRead;
        private readonly ICodingSession _codingSession;
        private readonly IFormFactory _formFactory;


        public MainPage(IApplicationLogger applogger, IFormController formController, IPanelColorControl panelControl, IErrorHandler errorHandler, IDatabaseSessionRead databaseRead, ICodingSession codingSession, IFormFactory formFactory)
        {
            InitializeComponent();
            _appLogger = applogger;
            _formController = formController;
            _panelColorControl = panelControl;
            _errorHandler = errorHandler;
            _databaseRead = databaseRead;
            _codingSession = codingSession;
            _formFactory = formFactory;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void MainPageCodingSessionButton_Click(object sender, EventArgs e)
        {
            _formController.HandleAndShowForm(_formFactory.CreateCodingSessionPage, nameof(CodingSessionPage));
        }


        private void MainPageEditSessionsButton_Click(object sender, EventArgs e)
        {
            _formController.HandleAndShowForm(_formFactory.CreateEditSessionPage, nameof(EditSessionPage));
        }

        private void MainPageViewSessionsButton_Click(object sender, EventArgs e)
        {
            _formController.HandleAndShowForm(_formFactory.CreateViewSessionsPage, nameof(ViewSessionsPage));
        }

        private void MainPageSettingsButton_Click(object sender, EventArgs e)
        {
            _formController.HandleAndShowForm(_formFactory.CreateSettingsPage, nameof(SettingsPage));
        }


        private void UpdateLabels(Panel parentPanel)
        {
            {
                List<DateTime> last28Days = _codingSession.GetDatesPrevious28days();
                var labels = parentPanel.Controls.OfType<Label>().ToList();
                for (int i = 0; i < last28Days.Count && i < labels.Count; i++)
                {
                    labels[i].Text = last28Days[i].ToShortDateString();
                }
            }
        }

        private void UpdateGradientPanels(Panel parentPanel)
        {
            List<DateTime> last28Days = _codingSession.GetDatesPrevious28days();
            List<int> sessionDurations = _databaseRead.ReadSessionDurationMinutes(28);

            var labels = parentPanel.Controls.OfType<Label>().ToList();
            for (int i = 0; i < last28Days.Count && i < labels.Count; i++)
            {
                int duration = sessionDurations.ElementAtOrDefault(i);
                SessionColor sessionColor = _panelColorControl.DetermineSessionColor(duration);
                Color color = _panelColorControl.GetColorFromSessionColor(sessionColor);

                labels[i].BackColor = color;
                labels[i].Text = last28Days[i].ToShortDateString();
            }
        }
    }
}
