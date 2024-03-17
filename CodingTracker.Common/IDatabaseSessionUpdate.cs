﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.IDatabaseSessionUpdates
{
    public interface IDatabaseSessionUpdate
    {
        void UpdateSession();
        void UpdateProgress();

    }
}
