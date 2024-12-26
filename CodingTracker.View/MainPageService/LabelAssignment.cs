using CodingTracker.Common.DataInterfaces.ICodingSessionRepositories;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Business.CodingSessionService.SessionCalculators;
using Guna.UI2.WinForms;

namespace CodingTracker.Business.MainPageService.LabelAssignments
{
    public interface ILabelAssignment
    {
        Task UpdateTodayLabel(Guna2HtmlLabel TodaySessionLabel);
    }

    public class LabelAssignment
    {
        private readonly IApplicationLogger _appLogger;
        private readonly ICodingSessionRepository _codingSessionRepository;
        private readonly ISessionCalculator _sessionCalculator;


        public LabelAssignment(IApplicationLogger appLogger, ICodingSessionRepository codingSessionRepository, ISessionCalculator sessionCalculator)
        {
            _appLogger = appLogger;
            _codingSessionRepository = codingSessionRepository;
            _sessionCalculator = sessionCalculator;
        }


        public async Task UpdateTodayLabel(Guna2HtmlLabel TodaySessionLabel)
        {
            if(! await _codingSessionRepository.CheckTodayCodingSessions())
            {
                TodaySessionLabel.Text = "Today's Total: 0";
                return;
            }
            double todayTotal = await _sessionCalculator.GetTodayTotalSession();
            TodaySessionLabel.Text = $"Today's Total: {todayTotal}";

        }


    }
}
