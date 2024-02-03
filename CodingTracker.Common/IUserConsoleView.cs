using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.IUserConsoleView
{
    public interface IUserConsoleView
    {
        public void DisplayStartOptions();
        public void DisplayViewOptions();
        public string GetViewSpecificOptions();

    }
}