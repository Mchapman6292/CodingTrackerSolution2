using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Logging.AcitivtyExtensions
{
    public static class ActivityExtension
    {
        public static string GetSpanId(this Activity activity) => activity?.SpanId.ToHexString() ?? string.Empty;
        public static string GetTraceId(this Activity activity) => activity?.TraceId.ToHexString() ?? string.Empty;
        public static string GetParentId(this Activity activity) => activity?.ParentSpanId.ToHexString() ?? string.Empty;
    }
}