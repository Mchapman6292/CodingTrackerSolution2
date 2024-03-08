using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.IDatabaseSessionReads
{
    public interface IDatabaseSessionRead
    {
        Task<List<int>> ReadSessionDurationMinutesAsync();
        Task ViewAllSession(bool partialView = false);
        Task ViewSpecific(string chosenDate);
        Task FilterSessionsByDay(string date, bool isDescending);
        Task FilterSessionsByWeek(string date, bool isDescending);
        Task FilterSessionsByYear(string year, bool isDescending);


    }
}
