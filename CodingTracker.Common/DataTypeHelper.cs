using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.CodingSessions;
using CodingTracker.Common.IApplicationLoggers;
using SQLitePCL;

namespace CodingTracker.Common.Helpers
{


    public class DataTypeHelper 
    {



        public DataTypeHelper(IApplicationLogger appLogger)
        {
           
        }


        public static string ConvertDurationSecondsIntoStringHHMM(CodingSession session, IApplicationLogger appLogger, string traceId)
        {
            appLogger.Info($"Starting {nameof(ConvertDurationSecondsIntoStringHHMM)} for {session.DurationSeconds}, TraceId: {traceId}.");

            if (session.DurationSeconds == null)
            {
                appLogger.Warning($"DurationSeconds is null, TraceId: {traceId}.");
                return "00:00";
            }

            TimeSpan time = TimeSpan.FromSeconds(session.DurationSeconds.Value);
            return time.ToString(@"hh\:mm");

            }
        }



     
    }
}
