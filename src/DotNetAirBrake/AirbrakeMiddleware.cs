using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DotNetAirbrake
{
    public class AirbrakeMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IAirbrakeClient airbrakeClient;

        public AirbrakeMiddleware(
            RequestDelegate next,
            IAirbrakeClient airbrakeClient)
        {
            this.next = next;
            this.airbrakeClient = airbrakeClient;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next.Invoke(context);
            }
            catch (Exception exc)
            {
                await this.airbrakeClient.SendAsync(exc, context);
                throw;
            }
        }
    }
}