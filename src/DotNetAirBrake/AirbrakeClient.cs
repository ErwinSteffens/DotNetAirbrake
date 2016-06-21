using System;
using System.Threading.Tasks;
using DotNetAirBrake.Builder;
using DotNetAirBrake.Models;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DotNetAirBrake
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

            if (string.IsNullOrEmpty(serverUrl))
            {
                throw new InvalidOperationException($"{nameof(serverUrl)} is empty in AirBrake options");
            }
            if (string.IsNullOrEmpty(projectId))
            {
                throw new InvalidOperationException($"{nameof(projectId)} is empty in AirBrake options");
            }
            if (string.IsNullOrEmpty(projectKey))
            {
                throw new InvalidOperationException($"{nameof(projectKey)} is empty in AirBrake options");
            }

            this.InitializeClient(serverUrl, projectId, projectKey);
        }

        public async Task SendAsync(Exception exception)
        {
            var notice = this.builder.Create(exception);
            await this.SendAsync(notice);
        }
        
        public async Task SendAsync(AirbrakeCreateNoticeMessage notice)
        {
            try
            {
                var airbrakeResponse = await this.client.PostJsonAsync(notice).
                                                  ReceiveJson<AirbrakeCreateNoticeResponse>();
                if (this.log.IsEnabled(LogLevel.Debug))
                {
                    this.log.LogDebug($"Airbrake notice created. Id: {airbrakeResponse.Id}, Url: {airbrakeResponse.Url}");
                }
            }
            catch (FlurlHttpException exc)
            {
                this.log.LogError("Failed to send exception to airbrake.", exc);
            }
        }

        private void InitializeClient(string serverUrl, string projectId, string projectKey)
        {
            var url = serverUrl.AppendPathSegments("api/v3/projects").
                                AppendPathSegment(projectId).
                                AppendPathSegment("notices").
                                SetQueryParam("key", projectKey);

            this.client = new FlurlClient(url);
            this.client.Settings.JsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
            { 
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}