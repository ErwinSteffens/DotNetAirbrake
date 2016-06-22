using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using DotNetAirbrake.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.PlatformAbstractions;

namespace DotNetAirbrake.Builder
{
    public class AirbrakeMessageBuilder : IAirbrakeMessageBuilder
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public AirbrakeMessageBuilder(
            IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;

            // TODO: Add methods for:
            // * Body data in params
            // * Action data (routing params, controller info) in params
            // * Add more environment data
            // * Add machine hostname to context
        }

        public AirbrakeCreateNoticeMessage Create(IEnumerable<Exception> exceptions, HttpContext context)
        {
            var notice = new AirbrakeCreateNoticeMessage();

            this.AddExceptions(notice, exceptions);
            this.AddContext(notice, context);
            this.AddFormParameters(notice, context);
            this.AddHeaders(notice, context);
            this.AddNotifier(notice);
            this.AddQueryParams(notice, context);
            this.AddSession(notice, context);

            return notice;
        }

        public AirbrakeCreateNoticeMessage Create(Exception exception, HttpContext context)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var exceptions = new[]
            {
                exception
            };
            return this.Create(exceptions, context);
        }

        public void AddSession(AirbrakeCreateNoticeMessage message, HttpContext context)
        {
            var session = AirbrakeMessageBuilder.TryGet(() => context.Session);
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

        public void AddQueryParams(AirbrakeCreateNoticeMessage message, HttpContext context)
        {
            var queryStringValues = QueryHelpers.ParseQuery(context.Request.QueryString.Value);
            foreach (var value in queryStringValues)
            {
                message.Params.Add(value.Key, value.Value.ToString());
            }
        }

        public void AddNotifier(AirbrakeCreateNoticeMessage message)
        {
            var assemblyName = typeof(AirbrakeClient).GetTypeInfo().Assembly.GetName();

            message.Notifier = new AirbrakeNotifier
            {
                Name = assemblyName.Name,
                Url = "https://rovecom.nl",
                Version = assemblyName.Version.ToString()
            };
        }

        public void AddHeaders(AirbrakeCreateNoticeMessage message, HttpContext context)
        {
            foreach (var header in context.Request.Headers)
            {
                var key = $"HTTP_{header.Key.ToUndercase()}";
                message.Environment.Add(key, header.Value.ToString());
            }
        }

        public void AddFormParameters(AirbrakeCreateNoticeMessage message, HttpContext context)
        {
            var form = AirbrakeMessageBuilder.TryGet(() => context.Request.Form);
            if (form != null)
            {
                foreach (var item in form)
                {
                    message.Params.Add(item.Key, item.Value.ToString());
                }
            }
        }

        private void AddExceptions(AirbrakeCreateNoticeMessage notice, IEnumerable<Exception> exceptions)
        {
            var errors = exceptions.Select(this.CreateError);
            notice.Errors = errors;
        }

        private void AddContext(AirbrakeCreateNoticeMessage notice, HttpContext context)
        {
            var request = context.Request;
            var user = context.User;
            var runtimeEnvironment = PlatformServices.Default.Runtime;
            var applicationEnvironment = PlatformServices.Default.Application;

            notice.Context = new AirbrakeContext
            {
                Environment = this.hostingEnvironment.EnvironmentName,
                Url = request.GetDisplayUrl(),
                UserAgent = request.Headers["User-Agent"],
                Version = applicationEnvironment.ApplicationVersion,
                RootDirectory = this.hostingEnvironment.WebRootPath,
                Os = $"{runtimeEnvironment.OperatingSystem} {runtimeEnvironment.OperatingSystemVersion}",
                Language = $"{runtimeEnvironment.RuntimeType} {runtimeEnvironment.RuntimeVersion}",
                UserId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                UserName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                UserEmail = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
            };
        }

        private AirbrakeError CreateError(Exception exception)
        {
            var error = new AirbrakeError
            {
                Type = exception.GetType().FullName,
                Message = exception.GetType().Name + ": " + exception.Message,
                Backtrace = this.CreateBacktrace(exception)
            };
            return error;
        }

        private IEnumerable<AirbrakeBacktraceItem> CreateBacktrace(Exception exception)
        {
            var stackTrace = new StackTrace(exception, true);
            var frames = stackTrace.GetFrames();
            if (frames == null || frames.Length == 0)
            {
                return new[] { new AirbrakeBacktraceItem { File = "none" } };
            }

            var lines = new List<AirbrakeBacktraceItem>();
            foreach (var frame in frames)
            {
                var method = frame.GetMethod();
                var function = method.Name;
                var column = frame.GetFileColumnNumber();

                var line = frame.GetFileLineNumber();
                if (line == 0)
                {
                    line = frame.GetILOffset();
                }

                var file = frame.GetFileName();
                if (!string.IsNullOrEmpty(file))
                {
                    file = Path.GetFileName(file);
                }
                else
                {
                    file = method.Name ?? "(unknown)";
                }

                lines.Add(new AirbrakeBacktraceItem
                {
                    File = file,
                    Column = column,
                    Function = function,
                    Line = line
                });
            }

            return lines.ToArray();
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