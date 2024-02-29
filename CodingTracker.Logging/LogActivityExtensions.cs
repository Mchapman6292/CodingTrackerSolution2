using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Logging.AcitivtyExtensions
{
    /// <summary>
    /// Provides extension methods for Systems.Diagnostics.Activity class  to extract and format tracing identifiers such as SpanId, TraceId, and ParentId.
    /// </summary>
    public static class ActivityExtension
    {
        // Gets the SpanId of the activity in hexadecimal string format.
        public static string GetSpanId(this Activity activity) => activity?.SpanId.ToHexString() ?? string.Empty;
        // Gets the TraceId of the activity in hexadecimal string format.
        public static string GetTraceId(this Activity activity) => activity?.TraceId.ToHexString() ?? string.Empty;
        // Gets the ParentId of the activity in hexadecimal string format. Useful for tracing logs across different components or services
        public static string GetParentId(this Activity activity) => activity?.ParentSpanId.ToHexString() ?? string.Empty;
    }
}