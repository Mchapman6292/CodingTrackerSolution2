using CodingTracker.Common.IApplicationControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Business.ApplicationControls
{
    public class ApplicationControl : IApplicationControl
    {
        public bool ApplicationIsRunning { get; private set; }


        public ApplicationController()
        {
            ApplicationIsRunning = false; // Set to false instead of true to ensure that processes don't run or exit prematurely or unintentionally.
        }

        public void StartApplication()
        {
            ApplicationIsRunning = true;
        }

        public void ExitApplication() 
        {
            ApplicationIsRunning = false; 
        }
    }
}