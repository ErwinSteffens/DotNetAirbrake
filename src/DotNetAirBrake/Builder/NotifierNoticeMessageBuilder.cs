using System.Reflection;
using DotNetAirBrake.Models;

namespace DotNetAirBrake.Builder
{
    internal class NotifierNoticeMessageBuilder : INoticeMessageBuilder
    {
        public void Build(AirbrakeCreateNoticeMessage message)
        {
            var assemblyName = typeof(AirbrakeClient).GetTypeInfo().Assembly.GetName();

            message.Notifier = new AirbrakeNotifier
            {
                Name = assemblyName.Name,
                Url = "https://rovecom.nl",
                Version = assemblyName.Version.ToString()
            };
        }
    }
}