using System;
using System.Linq;
using System.Text;
using DotNetAirBrake.Models;
using Microsoft.AspNetCore.Http;

namespace DotNetAirBrake.Builder
{
    internal class SessionNoticeMessageBuilder : INoticeMessageBuilder
    {
        private readonly IHttpContextAccessor contextAccessor;

        public SessionNoticeMessageBuilder(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public void Build(AirbrakeCreateNoticeMessage message)
        {
            var httpContext = this.contextAccessor.HttpContext;
            var session = this.GetSession(httpContext);
            if ((session == null) || !session.Keys.Any())
            {
                return;
            }

            foreach (var key in session.Keys)
            {
                if (key != null)
                {
                    byte[] value;
                    if (session.TryGetValue(key, out value))
                    {
                        if (value != null)
                        {
                            var stringValue = Encoding.UTF8.GetString(value);
                            message.Session.Add(key, stringValue);
                        }
                    }
                }
            }
        }

        private ISession GetSession(HttpContext httpContext)
        {
            ISession session = null;
            try
            {
                session = httpContext.Session;
            }
            catch (Exception)
            {
                // ignored
            }
            return session;
        }
    }
}