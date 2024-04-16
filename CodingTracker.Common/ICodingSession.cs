using CodingTracker.Common.CodingSessionDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.ICodingSessions
{
    public interface ICodingSession
    {
        void StartSession();
        void EndSession();
        bool CheckIfCodingSessionActive();

        List<DateTime> GetDatesPrevious28days();





    }
}
