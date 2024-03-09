using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System

using CodingTracker.Common.CodingSessionDTOs;

namespace CodingTracker.Common.IDatabaseSessionReads
{
    public interface IDatabaseSessionRead
    {
        Task<List<int>> ReadSessionDurationMinutesAsync();
        Task<List<CodingSessionDTO>> ViewRecentSession(int numberOfSessions);

        Task<List<CodingSessionDTO>> ViewAllSession(bool partialView = false);
        Task<List<CodingSessionDTO>> ViewSpecific(string chosenDate);
        Task<List<CodingSessionDTO>> FilterSessionsByDay(string date, bool isDescending);
        Task<List<CodingSessionDTO>> FilterSessionsByWeek(string date, bool isDescending);
        Task<List<CodingSessionDTO>> FilterSessionsByYear(string year, bool isDescending);


    }
}
