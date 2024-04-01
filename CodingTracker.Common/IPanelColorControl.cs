using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.IPanelColorControls
{
    public enum SessionColor
    {
        Grey,        // For 0 minutes
        RedGrey,     // For less than 60 minutes
        Red,         // For 1 to less than 2 hours
        Yellow,      // For 2 to less than 3 hours
        Green        // For 3 hours and more
    }
    public interface IPanelColorControl
    {
        SessionColor DetermineSessionColor(int sessionDurationMinutes);

        Color GetColorFromSessionColor(SessionColor color);
    }
}
