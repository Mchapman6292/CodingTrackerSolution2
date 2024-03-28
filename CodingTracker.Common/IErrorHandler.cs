using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.IErrorHandlers
{

    public interface IErrorHandler
    {
        void CatchErrorsAndLogWithStopwatch(Action action, string methodName, bool isDatabaseOperation = false);
    }
}