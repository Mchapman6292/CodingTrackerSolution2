using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.ICodingSessionTimers
{
    public interface ICodingSessionTimer
    {
        void StartCodingSessionTimer();
        void EndCodingSessionTimer();

    }
}
