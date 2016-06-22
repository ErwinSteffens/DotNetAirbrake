using System;
using System.Threading.Tasks;
using DotNetAirbrake.Models;

namespace DotNetAirbrake
{
    public interface IAirbrakeClient
    {
        /// <summary>
        ///     Sends the specified exception to Airbrake, the async way.
        /// </summary>
        /// <param name="exception">The exception.</param>
        Task SendAsync(Exception exception);

        /// <summary>
        ///     Sends the specified notice to Airbrake, the async way.
        /// </summary>
        /// <param name="notice">The notice.</param>
        Task SendAsync(AirbrakeCreateNoticeMessage notice);
    }
}