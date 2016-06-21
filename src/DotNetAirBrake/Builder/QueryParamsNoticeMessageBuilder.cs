using DotNetAirBrake.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace DotNetAirBrake.Builder
{
    internal class QueryParamsNoticeMessageBuilder : INoticeMessageBuilder
    {
        private readonly IHttpContextAccessor contextAccessor;

        public QueryParamsNoticeMessageBuilder(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public void Build(AirbrakeCreateNoticeMessage message)
        {
            var httpContext = this.contextAccessor.HttpContext;
            var queryStringValues = QueryHelpers.ParseQuery(httpContext.Request.QueryString.Value);
            foreach (var value in queryStringValues)
            {
                message.Params.Add(value.Key, value.Value.ToString());
            }
        }
    }
}