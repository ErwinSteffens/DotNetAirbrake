using System;
using DotNetAirbrake.Models;

namespace DotNetAirbrake.Builder
{
    public interface IAirbrakeMessageBuilder
    {
        /// <summary>
        ///     Creates a <see cref="AirbrakeCreateNoticeMessage" /> from the the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        AirbrakeCreateNoticeMessage Create(AirbrakeError error);

        /// <summary>
        ///     Creates a <see cref="AirbrakeCreateNoticeMessage" /> from the the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        ///     A <see cref="AirbrakeCreateNoticeMessage" />, created from the the specified exception.
        /// </returns>
        AirbrakeCreateNoticeMessage Create(Exception exception);
    }
}