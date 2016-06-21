using DotNetAirBrake.Models;

namespace DotNetAirBrake.Builder
{
    public interface INoticeMessageBuilder
    {
        void Build(AirbrakeCreateNoticeMessage message);
    }
}