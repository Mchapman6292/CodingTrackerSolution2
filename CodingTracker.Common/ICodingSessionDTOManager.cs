using CodingTracker.Common.CodingSessionDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.ICodingSessionDTOProviders
{
    public interface ICodingSessionDTOManager
    {
        CodingSessionDTO CreateCodingSessionDTO();

        void SetSessionStartTimeAndDate();
        void SetSessionEndTimeAndDate();
        int CalculateDurationMinutes();


        CodingSessionDTO GetOrCreateCurrentSessionDTO();

        CodingSessionDTO CreateAndReturnCurrentSessionDTO();


        void UpdateCurrentSessionDTO(int sessionId, int userId, DateTime? startTime = null, DateTime? endTime = null, DateTime? startDate = null, DateTime? endDate = null, int? durationMinutes = null);

    }
}
