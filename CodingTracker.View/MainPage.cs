using System.Data;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.View.FormControllers;
using CodingTracker.Common.IErrorHandlers;
using CodingTracker.Common.IDatabaseSessionReads;
using CodingTracker.Common.ICodingSessions;
using CodingTracker.View.FormFactories;
using CodingTracker.View.FormSwitchers;
using CodingTracker.Business.PanelColorControls;
using CodingTracker.Business.SessionCalculators;
using Guna.UI2.WinForms;
using System.Diagnostics;



namespace CodingTracker.View
{
    public partial class MainPage : Form
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IFormController _formController;
        private readonly IPanelColorControl _panelColorControl;
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabaseSessionRead _databaseRead;
        private readonly ICodingSession _codingSession;
        private readonly IFormFactory _formFactory;
        private readonly IFormSwitcher _formSwitcher;
        private readonly ISessionCalculator _sessionCalculator;



        public MainPage(IApplicationLogger applogger, IFormController formController, IPanelColorControl panelControl, IErrorHandler errorHandler, IDatabaseSessionRead databaseRead, ICodingSession codingSession, IFormFactory formFactory, IFormSwitcher formSwitcher = null, ISessionCalculator sessionCalculator = null)
        {
            InitializeComponent();
            _appLogger = applogger;
            _formController = formController;
            _panelColorControl = panelControl;
            _errorHandler = errorHandler;
            _databaseRead = databaseRead;
            _codingSession = codingSession;
            _formFactory = formFactory;
            _formSwitcher = formSwitcher;
            _sessionCalculator = sessionCalculator;



        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            UpdateLabels(Last28DaysPanel);
            UpDateLast28Days(Last28DaysPanel);
        }

        private void MainPageCodingSessionButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            _formSwitcher.SwitchToCodingSessionPage();
        }

        private void MainPageCodingSessionButton_MouseEnter(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2GradientButton btn = sender as Guna.UI2.WinForms.Guna2GradientButton;
            btn.FillColor = Color.FromArgb(94, 148, 255);
            btn.FillColor2 = Color.FromArgb(255, 77, 165);
        }

        private void MainPageCodingSessionButton_MouseLeave(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2GradientButton btn = sender as Guna.UI2.WinForms.Guna2GradientButton;
            btn.FillColor = Color.FromArgb(35, 34, 50);
            btn.FillColor2 = Color.FromArgb(35, 34, 50);
        }



        private void MainPageEditSessionsButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            _formSwitcher.SwitchToEditSessionPage();
        }

        private void MainPageSettingsButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            _formSwitcher.SwitchToSettingsPage();
        }


        private void UpdateLabels(Panel parentPanel)
        {
            _appLogger.Debug("UpdateLabels method started.");
            try
            {
                List<DateTime> last28Days = _codingSession.GetDatesPrevious28days();
                var GunaLabels = parentPanel.Controls.OfType<Guna.UI2.WinForms.Guna2HtmlLabel>().ToList();
                for (int i = 0; i < last28Days.Count && i < GunaLabels.Count; i++)
                {
                    GunaLabels[i].Text = last28Days[i].ToShortDateString();
                }
                _appLogger.Debug("UpdateLabels method completed successfully.");
            }
            catch (Exception ex)
            {
                _appLogger.Error($"An error occurred in UpdateLabels: {ex.Message}");
            }
        }

        private void UpDateLast28Days(Panel parentPanel)
        {
            _appLogger.Debug("UpDateLast28Days method started.");
            try
            {
                List<DateTime> last28Days = _codingSession.GetDatesPrevious28days();
                List<double> sessionDurations = _databaseRead.ReadSessionDurationSeconds(28);
                var gradientPanels = parentPanel.Controls.OfType<Guna.UI2.WinForms.Guna2GradientPanel>().ToList();
                for (int i = 0; i < last28Days.Count && i < gradientPanels.Count; i++)
                {
                    double duration = sessionDurations.ElementAtOrDefault(i);
                    SessionColor sessionColor = _panelColorControl.DetermineSessionColor(duration);
                    Color color = _panelColorControl.ConvertSessionColorToColor(sessionColor);

                    gradientPanels[i].BackColor = color;
                    gradientPanels[i].Tag = last28Days[i].ToShortDateString();

                    var label = gradientPanels[i].Controls.OfType<Label>().FirstOrDefault();
                    if (label != null)
                    {
                        label.Text = last28Days[i].ToShortDateString();
                    }

                    _appLogger.Debug($"Panel {i}: Date {last28Days[i].ToShortDateString()}, Duration {duration}, SessionColor {sessionColor}, RGB ({color.R}, {color.G}, {color.B})");
                }
                _appLogger.Debug("UpDateLast28Days method completed successfully.");
            }
            catch (Exception ex)
            {
                _appLogger.Error($"An error occurred in UpDateLast28Days: {ex.Message}");
            }
        }

        private void UpdatedateTodaySessionLabel() 
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            using(var activity = new Activity(nameof(UpdatedateTodaySessionLabel))) 
            {

            }
        }

        private void Day2Label_Click(object sender, EventArgs e)
        {

        }
    }
}
 