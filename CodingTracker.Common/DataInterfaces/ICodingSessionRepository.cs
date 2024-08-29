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
        Task<bool> AddCodingSession(CodingSession codingSession, Activity activity);

        Task<IEnumerable<CodingSession>> GetSessionsByDateRange(DateOnly startDate, DateOnly endDate, string traceId);

        Task<(bool success, List<int> failedIds)> DeleteSessionsById(List<int> sessionIds, string traceId);

        Task<IEnumerable<CodingSession>> GetCodingSessionByDateOnly(DateOnly startDate, DateOnly endDate, string traceId);

        Task<bool> SaveCodingSessionChanges(Activity activity);






    }
}
