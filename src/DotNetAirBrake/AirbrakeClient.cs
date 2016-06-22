using System;
using System.Threading.Tasks;
using DotNetAirbrake.Builder;
using DotNetAirbrake.Models;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DotNetAirbrake
{
    public class AirbrakeClient : IAirbrakeClient
    {
        private readonly IAirbrakeMessageBuilder builder;
        private readonly ILogger log;
        private FlurlClient client;

        public AirbrakeClient(
            IAirbrakeMessageBuilder messageBuilder,
            ILoggerFactory loggerFactory,
            IOptions<AirbrakeOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.log = loggerFactory.CreateLogger<AirbrakeClient>();
            this.builder = messageBuilder;

            var serverUrl = options.Value.Url;
            var projectId = options.Value.ProjectId;
            var projectKey = options.Value.ProjectKey;

            this.Init(serverUrl, projectId, projectKey);
        }

        public AirbrakeClient(
            IAirbrakeMessageBuilder messageBuilder,
            ILoggerFactory loggerFactory,
            string serverUrl,
            string projectId,
            string projectKey)
        {
            this.log = loggerFactory.CreateLogger<AirbrakeClient>();
            this.builder = messageBuilder;

            this.Init(serverUrl, projectId, projectKey);
        }

        public async Task SendAsync(Exception exception, HttpContext context)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var notice = this.builder.Create(exception, context);
            await this.SendAsync(notice);
        }

        public async Task SendAsync(AirbrakeCreateNoticeMessage notice)
        {
            if (notice == null)
            {
                throw new ArgumentNullException(nameof(notice));
            }

            try
            {
                var airbrakeResponse = await this.client.
                                                  PostJsonAsync(notice).
                                                  ReceiveJson<AirbrakeCreateNoticeResponse>();
                if (this.log.IsEnabled(LogLevel.Debug))
                {
                    this.log.LogDebug($"Airbrake notice created. Id: {airbrakeResponse.Id}, Url: {airbrakeResponse.Url}");
                }
            }
            catch (FlurlHttpException exc)
            {
                this.log.LogError("Failed to send exception to Airbrake service", exc);
            }
        }

        private void Init(string serverUrl, string projectId, string projectKey)
        {
            if (string.IsNullOrEmpty(projectKey))
            {
                throw new InvalidOperationException($"{nameof(projectKey)} is empty in Airbrake options");
            }
            if (string.IsNullOrEmpty(projectId))
            {
                throw new InvalidOperationException($"{nameof(projectId)} is empty in Airbrake options");
            }
            if (string.IsNullOrEmpty(serverUrl))
            {
                throw new InvalidOperationException($"{nameof(serverUrl)} is empty in Airbrake options");
            }

            var url = serverUrl.AppendPathSegments("api/v3/projects").
                                AppendPathSegment(projectId).
                                AppendPathSegment("notices").
                                SetQueryParam("key", projectKey);

            this.client = new FlurlClient(url)
            {
                Settings =
                {
                    JsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })
                }
            };
        }
    }
}