using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.IApplicationControls
{
    public interface IApplicationControl
    {
        void StartApplication();
        Task ExitApplication();
    }
}
