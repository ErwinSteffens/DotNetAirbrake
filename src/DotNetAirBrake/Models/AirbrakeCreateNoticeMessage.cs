using System.Collections.Generic;

namespace DotNetAirbrake.Models
{
    public class AirbrakeCreateNoticeMessage
    {
        public AirbrakeNotifier Notifier { get; set; }
        public AirbrakeContext Context { get; set; }
        public IEnumerable<AirbrakeError> Errors { get; set; }
        public IDictionary<string, string> Environment { get; set; }
        public IDictionary<string, string> Session { get; set; }
        public IDictionary<string, string> Params { get; set; }
    }
}