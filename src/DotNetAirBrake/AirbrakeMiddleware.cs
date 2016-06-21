using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DotNetAirBrake
{
    public class AirbrakeMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IAirbrakeClient airbrakeClient;
        private readonly ILogger log;

        public AirbrakeMiddleware(
            RequestDelegate next,
            IAirbrakeClient airbrakeClient,
            ILoggerFactory loggerFactory)
        {
            this.next = next;
            this.airbrakeClient = airbrakeClient;
            this.log = loggerFactory.CreateLogger<AirbrakeMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            Exception exceptionToSend = null;
            try
            {
                await this.next.Invoke(context);
            }
            catch (Exception e)
            {
                exceptionToSend = e;
                await this.SendExceptionAsync(exceptionToSend);
            }

            if (exceptionToSend != null)
            {
                await this.SendExceptionAsync(exceptionToSend);
                throw exceptionToSend;
            }
        }

        private async Task SendExceptionAsync(Exception exceptionToSend)
        {
            try
            {
                await this.airbrakeClient.SendAsync(exceptionToSend);
            }
            catch (Exception exc)
            {
                this.log.LogError("Failed to send exception to airbrake", exc);
            }
        }
    }
}