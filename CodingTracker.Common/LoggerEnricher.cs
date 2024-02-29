using Serilog;
using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;
using Serilog.Enrichers.Span;

namespace CodingTracker.Common.LoggerEnrichers
{
    public class LoggerEnricher : ILoggerEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var activity = Activity.Current;
            if (activity != null)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", activity.GetSpanId()));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", activity.GetTraceId()));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ParentId", activity.GetParentId()));
            }
        }
    }
}