using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Common.IApplicationLoggers;

namespace CodingTracker.Common.IdGenerators
{
    public interface IIdGenerators
    {
        int GenerateUserId(Activity activity);
        int GenerateSessionId(Activity activity);
    }

    public  class IdGenerators : IIdGenerators
    {
        private int lastAssignedUserId = 0;
        private int LastAssignedSessionId = 0;
        private readonly IApplicationLogger _appLogger;



        public IdGenerators(IApplicationLogger appLogger) 
        {
            _appLogger = appLogger ?? throw new ArgumentNullException(nameof(appLogger));
        }




        public int GenerateUserId(Activity activity)
        {
            if(activity == null) throw new ArgumentNullException(nameof(activity));

            _appLogger.Info($"Starting {nameof(GenerateUserId)} TraceId : {activity.TraceId}");

            int newId = ++ lastAssignedUserId;

            _appLogger.Info($"LastAssignedUserId updated to {lastAssignedUserId}, new userId = {newId} TraceId : {activity.TraceId}.");

            return newId;
        }


        public int GenerateSessionId(Activity activity)
        {
            if(activity == null) throw new ArgumentNullException (nameof(activity));

            _appLogger.Info($"Starting {nameof(GenerateSessionId)} TraceId : {activity.TraceId}");

            int newId = ++LastAssignedSessionId;

            _appLogger.Info($"LastAssignedSessionId updated to {LastAssignedSessionId}, new userId = {newId} TraceId : {activity.TraceId}.");

            return newId;
        }
    }
}
