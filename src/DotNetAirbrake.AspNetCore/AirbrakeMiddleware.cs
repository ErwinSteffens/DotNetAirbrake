using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DotNetAirbrake.AspNetCore
{
    public class AirbrakeMiddleware
    {
        private readonly RequestDelegate next;

        public AirbrakeMiddleware(
            RequestDelegate next)
        {
            this.next = next;
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