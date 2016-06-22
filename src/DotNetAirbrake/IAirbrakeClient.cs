using System;
using System.Threading.Tasks;
using DotNetAirbrake.Models;
using Microsoft.AspNetCore.Http;

namespace DotNetAirbrake
{
    public interface IAirbrakeClient
    {
        /// <summary>
        ///     Sends the specified exception to Airbrake, the async way.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="context">The http context.</param>
        Task SendAsync(Exception exception, HttpContext context);

        /// <summary>
        ///     Sends the specified notice to Airbrake, the async way.
        /// </summary>
        /// <param name="notice">The notice.</param>
        Task SendAsync(AirbrakeCreateNoticeMessage notice);
    }
}