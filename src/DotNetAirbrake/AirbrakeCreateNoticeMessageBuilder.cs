using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNetAirbrake.Models;
using Microsoft.Extensions.PlatformAbstractions;

namespace DotNetAirbrake
{
    public class AirbrakeCreateNoticeMessageBuilder : ObjectBuilder<AirbrakeCreateNoticeMessage>
    {
        public AirbrakeCreateNoticeMessageBuilder WithNotifier()
        {
            var assemblyName = typeof(AirbrakeClient).GetTypeInfo().Assembly.GetName();

            this.With(m =>
            {
                m.Notifier = new AirbrakeNotifier
                {
                    Name = assemblyName.Name,
                    Url = "https://rovecom.nl",
                    Version = assemblyName.Version.ToString()
                };
            });

            return this;
        }

        public AirbrakeCreateNoticeMessageBuilder WithException(Exception exception)
        {
            var exceptions = new[] { exception };
            var errors = exceptions.Select(CreateError);
            this.With(m => m.Errors = errors);

            return this;
        }

        public AirbrakeCreateNoticeMessageBuilder WithException(IEnumerable<Exception> exceptions)
        {
            var errors = exceptions.Select(CreateError);
            this.With(m => m.Errors = errors);

            return this;
        }

        public AirbrakeCreateNoticeMessageBuilder WithContext()
        {
            this.With(m =>
            {
                m.Context = new AirbrakeContext
                {
                    Version = PlatformServices.Default.Application.ApplicationVersion,
                    Os = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                    Language = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                };
            });

            return this;
        }

        private static AirbrakeError CreateError(Exception exception)
        {
            var error = new AirbrakeError
            {
                Type = exception.GetType().FullName,
                Message = exception.GetType().Name + ": " + exception.Message,
                Backtrace = CreateBacktrace(exception)
            };
            return error;
        }

        private static IEnumerable<AirbrakeBacktraceItem> CreateBacktrace(Exception exception)
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
    }
}