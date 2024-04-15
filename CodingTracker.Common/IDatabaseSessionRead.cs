using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.UserCredentialDTOs;

namespace CodingTracker.Common.IDatabaseSessionReads
{
    public interface IDatabaseSessionRead
    {
        List<int> ReadSessionDurationSeconds(int numberOfDays, bool readAll = false);

        List<UserCredentialDTO> ReadUserCredentials(bool returnLastLoggedIn);

        List<(DateTime Day, int TotalDurationSeconds)> ReadTotalSessionDurationByDay();
        int GetSessionIdWithMostRecentLogin();

        List<CodingSessionDTO> ViewAllSession(bool partialView = false);
        List<CodingSessionDTO> ViewSpecific(string chosenDate);
        List<CodingSessionDTO> ViewRecentSession(int numberOfSessions);


        List<CodingSessionDTO> FilterSessionsByDay(string date, bool isDescending);
        List<CodingSessionDTO> FilterSessionsByWeek(string date, bool isDescending);
        List<CodingSessionDTO> FilterSessionsByYear(string year, bool isDescending);

        void GetLast28DaysSessions();


    }
}
