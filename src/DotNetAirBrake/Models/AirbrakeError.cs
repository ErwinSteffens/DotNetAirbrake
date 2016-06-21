using System.Collections.Generic;

namespace DotNetAirBrake.Models
{
    public class AirbrakeError
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public IEnumerable<AirbrakeBacktraceItem> Backtrace { get; set; }
    }
}