using CodingTracker.Business.CodingSession;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Business
{
    public class CodingController
    {
        public void FilterSessionsByDay()
        {
            string CommandText = @"SELECT* FROM CodingSessions
            WHERE DATE(StartTime) = DATE('your-date-here')
            ORDER BY StartTime ASC; --Or DESC for descending";

        }

        public void FilterSessionsByWeek()
        {
            string CommandText = @"
                SELECT * FROM CodingSessions 
                WHERE strftime('%W', StartTime) = strftime('%W', 'your-date-here') AND 
                strftime('%Y', StartTime) = strftime('%Y', 'your-date-here') 
                ORDER BY StartTime ASC; -- Or DESC for descending";

        }


        public void FilterSessionsByYear()
        {
            string CommandText = @"
                SELECT * FROM CodingSessions 
                WHERE strftime('%Y', StartTime) = 'your-year-here' 
                ORDER BY StartTime ASC; -- Or DESC for descending";

        }




    }
}
