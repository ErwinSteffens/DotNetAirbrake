using System;
using System.Threading.Tasks;
using DotNetAirbrake.Models;

namespace DotNetAirbrake
{
    public static class AirbrakeExceptionExtensions
    {
        public static Task SendToAirbrakeAsync(this Exception exc, AirbrakeClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var notice = AirbrakeExceptionExtensions.BuildNoticeMessage(exc);
            return client.SendAsync(notice);
        }

        private static AirbrakeCreateNoticeMessage BuildNoticeMessage(Exception exc)
        {
            var builder = new AirbrakeCreateNoticeMessageBuilder();
            builder.WithNotifier();
            builder.WithContext();
            builder.WithException(exc);
            return builder.Build();
        }
    }
}