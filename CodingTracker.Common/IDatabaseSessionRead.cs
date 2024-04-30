using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.IQueryBuilders;
using CodingTracker.Common.UserCredentialDTOs;

namespace CodingTracker.Common.IDatabaseSessionReads
{
    public interface IDatabaseSessionRead
    {
        List<double> ReadSessionDurationSeconds(int numberOfDays, bool readAll = false);

        List<UserCredentialDTO> ReadUserCredentials(bool returnLastLoggedIn);

        List<(DateTime Date, double TotalDurationSeconds)> ReadDurationSecondsLast28Days();
        int GetUserIdWithMostRecentLogin();
        int GetSessionIdWithMostRecentLogin();

        List<CodingSessionDTO> ViewAllSession(bool partialView = false);
        List<CodingSessionDTO> ViewSpecific(DateTime chosenDate);
        List<CodingSessionDTO> ViewRecentSession(int numberOfSessions);


        List<CodingSessionDTO> SelectAllSessionsForDate(string date, bool isDescending);
        List<CodingSessionDTO> SelectAllSesssionsForWeek(string date, bool isDescending);
        List<CodingSessionDTO> SelectAllSessionsForYear(string year, bool isDescending);

        void GetLast28DaysSessions();



    }
}
