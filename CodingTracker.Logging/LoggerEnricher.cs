
using Serilog;
using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;
using Serilog.Enrichers.Span;
using CodingTracker.Common.ILoggerEnrichers;

namespace CodingTracker.Logging.LoggerEnrichers
{
    /// <summary>
    /// Automatically adds unique trace identifiers from the current System.Diagnostics.Activity to each log event
    /// </summary>
    public class LoggerEnricher : ILoggerEnricher
    {
        /// </summary>
        /// <param name="logEvent">The log event to be enriched.</param>
        /// <param name="propertyFactory">A factory that creates new log event properties.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var activity = Activity.Current;
            if (activity != null)
            {
                // TraceId & SpanId must be converted to string when used as parameters. ParentId is a string property of the Activity class, this directly returns the parent spans identifier  as a string. 

                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", activity.GetSpanId()));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", activity.GetTraceId()));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ParentId", activity.GetParentId()));
            }
        }
    }
}