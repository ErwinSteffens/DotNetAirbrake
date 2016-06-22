using DotNetAirbrake.Models;

namespace DotNetAirbrake.Builder
{
    public interface INoticeMessageBuilder
    {
        void Build(AirbrakeCreateNoticeMessage message);
    }
}