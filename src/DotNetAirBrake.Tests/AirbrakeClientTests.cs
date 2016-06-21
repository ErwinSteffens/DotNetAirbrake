using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using DotNetAirBrake;
using FluentAssertions.Execution;
using Flurl.Http;
using Flurl.Http.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
//using NJsonSchema;
using Xunit;
using Xunit.Abstractions;

namespace Geitenbelang.FeedStationApi.Api.Tests
{
    public class AirbrakeClientTests
    {
        private const string AirbrakeSchemaFile = "airbrake_v3.schema.json";

        private readonly ITestOutputHelper output;

        public AirbrakeClientTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async void SendAsync_WhenAllOptionsAreSet_ShouldSendValidJson()
        {
            using (var httpTest = new HttpTest())
            {
                // Arrange
                const string projectId = "MyProject";
                const string projectKey = "my-project-key";
                const string url = "https://exceptions.testurl.com";
                httpTest.RespondWithJson(200, new AirbrakeCreateNoticeResponse
                {
                    Id = 100,
                    Url = $"{url}/id/100"
                });
                var mockOptions = new Mock<IOptions<AirbrakeOptions>>();
                mockOptions.SetupGet(o => o.Value).Returns(new AirbrakeOptions
                {
                    ProjectId = projectId,
                    ProjectKey = projectKey,
                    ServerUrl = url
                });
                var mockLogger = new Mock<ILogger>();
                var mockLoggerFactory = new Mock<ILoggerFactory>();
                mockLoggerFactory.Setup(m => m.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
                var client = new AirbrakeClient(null, mockLoggerFactory.Object, mockOptions.Object);
                var notice = new AirbrakeCreateNoticeMessage
                {
                    Notifier = new AirbrakeNotifier
                    {
                        Name = "TestNotifier",
                        Url = "https://app.my-applications.nl",
                        Version = "1.0.0.0"
                    },
                    Errors = new[]
                    {
                        new AirbrakeError
                        {
                            Type = typeof(Exception).FullName,
                            Message = "MyErrorException",
                            Backtrace = new[]
                            {
                                new AirbrakeBacktraceItem
                                {
                                    File = "MyFile.cs",
                                    Column = 30,
                                    Line = 20,
                                    Function = "MyTestFunction"
                                }
                            }
                        }
                    },
                    Context = new AirbrakeContext
                    {
                        Environment = "Staging",
                        Url = "https://app.myapp.com",
                        Os = "Windows 10",
                        Version = "10.0.0",
                        Action = "DoSomething",
                        Component = "TheComponent",
                        Language = "nl",
                        RootDirectory = "/root",
                        UserAgent = "IE",
                        UserEmail = "mail@user.nl",
                        UserId = "123",
                        UserName = "User1",
                        UserUsername = "User name 1"
                    },
                    Environment = new Dictionary<string, string>
                    {
                        { "EnvKey1", "EnvValue1" },
                        { "EnvKey2", "EnvValue2" }
                    },
                    Params = new Dictionary<string, string>
                    {
                        { "ParamKey1", "ParamValue1" },
                        { "ParamKey2", "ParamValue2" }
                    },
                    Session = new Dictionary<string, string>
                    {
                        { "SessionKey1", "SessionValue1" },
                        { "SessionKey2", "SessionValue2" }
                    }
                };

                // Act
                await client.SendAsync(notice);

                // Assert
                httpTest.ShouldHaveCalled($"{url}/api/v3/projects/{projectId}/notices?key={projectKey}").
                         WithVerb(HttpMethod.Post).
                         WithContentType("application/json").
                         Times(1);
                var call = httpTest.CallLog.First();
                this.ValidateJsonBody(call);
            }
        }

        private void ValidateJsonBody(HttpCall call)
        {
            if (!File.Exists(AirbrakeClientTests.AirbrakeSchemaFile))
            {
                throw new InvalidOperationException($"Schema file '{AirbrakeClientTests.AirbrakeSchemaFile}' does not exist");
            }

            var jsonSchemaData = File.ReadAllText(AirbrakeClientTests.AirbrakeSchemaFile);
            var test = JsonSchema4.FromJson(jsonSchemaData);

            var requestBodyJson = JObject.Parse(call.RequestBody);
            var errors = test.Validate(requestBodyJson);
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    this.output.WriteLine($"Json schema error: {error.Path}, {error.Kind}");
                }
                throw new AssertionFailedException("Failed to validate json schema");
            }
        }
    }
}