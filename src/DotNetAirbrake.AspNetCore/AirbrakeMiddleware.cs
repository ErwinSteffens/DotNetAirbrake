using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DotNetAirbrake.AspNetCore
{
    public class AirbrakeMiddleware
    {
        private readonly RequestDelegate next;
        private readonly AirbrakeClient airbrakeClient;

        public AirbrakeMiddleware(
            RequestDelegate next,
            AirbrakeClient airbrakeClient)
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
                await exc.SendToAirbrakeAsync(context);
                throw;
            }
        }
    }
}