using DotNetAirbrake.Models;
using Microsoft.AspNetCore.Http;

namespace DotNetAirbrake.Builder
{
    internal class HeadersNoticeMessageBuilder : INoticeMessageBuilder
    {
        private readonly IHttpContextAccessor contextAccessor;

        public HeadersNoticeMessageBuilder(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public void Build(AirbrakeCreateNoticeMessage message)
        {
            var httpContext = this.contextAccessor.HttpContext;
            foreach (var header in httpContext.Request.Headers)
            {
                var key = $"HTTP_{this.ToUndercase(header.Key)}";
                message.Environment.Add(key, header.Value.ToString());
            }
        }

        private string ToUndercase(string key)
        {
            return key.Replace('-', '_');
        }
    }
}