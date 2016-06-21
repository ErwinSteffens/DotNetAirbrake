using System;
using DotNetAirBrake.Models;
using Microsoft.AspNetCore.Http;

namespace DotNetAirBrake.Builder
{
    internal class FormParamsNoticeMessageBuilder : INoticeMessageBuilder
    {
        private readonly IHttpContextAccessor contextAccessor;

        public FormParamsNoticeMessageBuilder(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public void Build(AirbrakeCreateNoticeMessage message)
        {
            var form = FormParamsNoticeMessageBuilder.TryGet(() =>
            {
                var httpContext = this.contextAccessor.HttpContext;
                return httpContext.Request.Form;
            });
            if (form != null)
            {
                foreach (var item in form)
                {
                    message.Params.Add(item.Key, item.Value.ToString());
                }
            }
        }

        private static T TryGet<T>(Func<T> getter) where T : class
        {
            try
            {
                return getter();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}