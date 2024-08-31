using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.CodingSessions;

namespace CodingTracker.Common.Interfaces.ICodingSessionRepository
{
    public interface ICodingSessionRepository

    {
        Task<bool> AddCodingSession(Activity activity, CodingSession session);

        Task<IEnumerable<CodingSession>> GetAllCodingSessions(Activity activity);


        Task<(bool success, List<int> failedIds)> DeleteSessionsById(Activity activity, List<int> sessionIds);

        Task<IEnumerable<CodingSession>> GetCodingSessionByDateOnly(Activity activity, DateOnly startDate, DateOnly endDate);

        Task<bool> SaveCodingSessionChanges(Activity activity);







    }
}
