using CodingTracker.Common.CodingSessionDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.IDatabaseSessionDeletes
{
    public interface IDatabaseSessionDelete
    {
        void DeleteSession(List<int> sessionIds);
    }
}
