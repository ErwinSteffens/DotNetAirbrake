using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetAirbrake
{
    public static class AirbrakeExceptionExtensions
    {
        public static Task SendToAirbrakeAsync(this Exception exc, HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var client = context.RequestServices.GetService<IAirbrakeClient>();
            return client.SendAsync(exc, context);
        }
    }
}