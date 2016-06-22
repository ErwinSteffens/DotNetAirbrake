using System;
using DotNetAirbrake.Models;
using Microsoft.AspNetCore.Http;

namespace DotNetAirbrake.Builder
{
    public interface IAirbrakeMessageBuilder
    {
        /// <summary>
        ///     Creates a <see cref="AirbrakeCreateNoticeMessage" /> from the the specified exception
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <param name="context">The http context</param>
        /// <returns>
        ///     A <see cref="AirbrakeCreateNoticeMessage" />, created from the the specified exception
        /// </returns>
        AirbrakeCreateNoticeMessage Create(Exception exception, HttpContext context);
    }
}