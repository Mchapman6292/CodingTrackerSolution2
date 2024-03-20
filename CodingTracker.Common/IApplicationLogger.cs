

using System.Diagnostics;

namespace CodingTracker.Common.IApplicationLoggers
{
    public interface IApplicationLogger
    {
        void LogActivity(string methodName, Action<Activity> logAction, Action action);
        void Info(string message);
        void Info(string message, params object[] propertyValues); 
        void Debug(string message);
        void Debug(string message, params object[] propertyValues); 
        void Warning(string message);
        void Warning(string message, params object[] propertyValues); 
        void Error(string message, Exception ex);
        void Error(string message, Exception ex, params object[] propertyValues);
        void Error(string message, params object[] propertyValues);
        void Fatal(string message);
        void Fatal(string message, Exception ex);

    }
}
