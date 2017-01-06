using System;
using System.Threading.Tasks;
using DotNetAirbrake.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetAirbrake.AspNetCore
{
    public static class AirbrakeExceptionHttpExtensions
    {
        public static Task SendToAirbrakeAsync(this Exception exc, HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var notice = AirbrakeExceptionHttpExtensions.BuildNoticeMessage(exc, context);

            var client = context.RequestServices.GetService<AirbrakeClient>();
            return client.SendAsync(notice);
        }

        private static AirbrakeCreateNoticeMessage BuildNoticeMessage(Exception exc, HttpContext httpContext)
        {
            var builder = new AirbrakeCreateNoticeMessageBuilder();
            builder.WithNotifier();
            builder.WithHttpContext(httpContext);
            builder.WithException(exc);
            return builder.Build();
        }
    }
}