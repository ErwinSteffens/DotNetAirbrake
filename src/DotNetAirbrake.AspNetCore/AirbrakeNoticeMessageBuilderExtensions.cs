using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using DotNetAirbrake.Extensions;
using DotNetAirbrake.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetAirbrake.AspNetCore
{
    public static class AirbrakeCreateNoticeMessageBuilderExtensions
    {
        public static AirbrakeCreateNoticeMessageBuilder WithHttpContext(this AirbrakeCreateNoticeMessageBuilder builder, HttpContext context)
        {
            builder.WithQueryParams(context)
            .WithFormParameters(context)
            .WithContext(context)
            .WithHeaders(context)
            .WithSession(context);
            return builder;
        }

        public static AirbrakeCreateNoticeMessageBuilder WithHeaders(this AirbrakeCreateNoticeMessageBuilder builder, HttpContext context)
        {
            builder.With(m =>
            {
                foreach (var header in context.Request.Headers)
                {
                    var key = $"HTTP_{header.Key.ToUndercase()}";
                    m.Environment.Add(key, header.Value.ToString());
                }
            });
            return builder;
        }

        public static AirbrakeCreateNoticeMessageBuilder WithSession(this AirbrakeCreateNoticeMessageBuilder builder, HttpContext context)
        {
            builder.With(m =>
            {
                var session = AirbrakeCreateNoticeMessageBuilderExtensions.TryGet(() => context.Session);
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
                                m.Session.Add(key, stringValue);
                            }
                        }
                    }
                }
            });
            return builder;
        }

        public static AirbrakeCreateNoticeMessageBuilder WithQueryParams(this AirbrakeCreateNoticeMessageBuilder builder, HttpContext context)
        {
            builder.With(m =>
            {
                var queryStringValues = QueryHelpers.ParseQuery(context.Request.QueryString.Value);
                foreach (var value in queryStringValues)
                {
                    m.Params.Add(value.Key, value.Value.ToString());
                }
            });
            return builder;
        }

        public static AirbrakeCreateNoticeMessageBuilder AddNotifier(this AirbrakeCreateNoticeMessageBuilder builder)
        {
            builder.With(m =>
            {
                var assemblyName = typeof(AirbrakeClient).GetTypeInfo().Assembly.GetName();

                m.Notifier = new AirbrakeNotifier
                {
                    Name = assemblyName.Name,
                    Url = "https://rovecom.nl",
                    Version = assemblyName.Version.ToString()
                };
            });
            return builder;
        }

        public static AirbrakeCreateNoticeMessageBuilder AddHeaders(this AirbrakeCreateNoticeMessageBuilder builder, HttpContext context)
        {
            builder.With(m =>
            {
                foreach (var header in context.Request.Headers)
                {
                    var key = $"HTTP_{header.Key.ToUndercase()}";
                    m.Environment.Add(key, header.Value.ToString());
                }
            });
            return builder;
        }

        public static AirbrakeCreateNoticeMessageBuilder WithFormParameters(this AirbrakeCreateNoticeMessageBuilder builder, HttpContext context)
        {
            builder.With(m =>
            {
                var form = AirbrakeCreateNoticeMessageBuilderExtensions.TryGet(() => context.Request.Form);
                if (form != null)
                {
                    foreach (var item in form)
                    {
                        m.Params.Add(item.Key, item.Value.ToString());
                    }
                }
            });
            return builder;
        }

        private static AirbrakeCreateNoticeMessageBuilder WithContext(this AirbrakeCreateNoticeMessageBuilder builder, HttpContext context)
        {
            var hostingEnvironment = context.RequestServices.GetService<IHostingEnvironment>();

            builder.With(m =>
            {
                var request = context.Request;
                var user = context.User;

                m.Context = new AirbrakeContext
                {
                    Environment = hostingEnvironment.EnvironmentName,
                    Url = request.GetDisplayUrl(),
                    UserAgent = request.Headers["User-Agent"],
                    Version = PlatformServices.Default.Application.ApplicationVersion,
                    RootDirectory = hostingEnvironment.WebRootPath,
                    Os = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                    Language = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                    UserId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                    UserName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                    UserEmail = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                };
            });
            return builder;
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