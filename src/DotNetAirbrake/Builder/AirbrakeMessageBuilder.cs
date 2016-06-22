using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DotNetAirbrake.Models;

namespace DotNetAirbrake.Builder
{
    public class AirbrakeMessageBuilder : IAirbrakeMessageBuilder
    {
        private readonly IEnumerable<INoticeMessageBuilder> builders;

        public AirbrakeMessageBuilder(
            IEnumerable<INoticeMessageBuilder> builders)
        {
            this.builders = builders;

            // TODO: Add builders for:
            // * Body data in params
            // * Action data (routing params, controller info) in params
            // * Add more environment data
        }

        public AirbrakeCreateNoticeMessage Create(AirbrakeError error)
        {
            return this.Create(new[] { error });
        }

        public AirbrakeCreateNoticeMessage Create(IEnumerable<AirbrakeError> errors)
        {
            var notice = new AirbrakeCreateNoticeMessage
            {
                Errors = errors,
                Environment = new Dictionary<string, string>(),
                Params = new Dictionary<string, string>(),
                Session = new Dictionary<string, string>()
            };

            foreach (var builder in this.builders)
            {
                builder.Build(notice);
            }

            return notice;
        }

        public AirbrakeCreateNoticeMessage Create(IEnumerable<Exception> exceptions)
        {
            var errors = exceptions.Select(this.BuildError);
            return this.Create(errors);
        }

        public AirbrakeCreateNoticeMessage Create(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var error = this.BuildError(exception);
            return this.Create(error);
        }

        private AirbrakeError BuildError(Exception exception)
        {
            var error = new AirbrakeError
            {
                Type = exception.GetType().FullName,
                Message = exception.GetType().Name + ": " + exception.Message,
                Backtrace = this.BuildBacktrace(exception)
            };
            return error;
        }

        private IEnumerable<AirbrakeBacktraceItem> BuildBacktrace(Exception exception)
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