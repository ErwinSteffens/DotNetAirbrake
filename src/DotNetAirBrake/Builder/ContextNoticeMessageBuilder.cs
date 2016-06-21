using System.Linq;
using System.Security.Claims;
using DotNetAirBrake.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.PlatformAbstractions;

namespace DotNetAirBrake.Builder
{
    internal class ContextNoticeMessageBuilder : INoticeMessageBuilder
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ContextNoticeMessageBuilder(
            IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.httpContextAccessor = httpContextAccessor;
        }

        public void Build(AirbrakeCreateNoticeMessage message)
        {
            // TODO: Fetch somehow the action and controller and add it to the context

            message.Context = this.CreateContext();
        }

        private AirbrakeContext CreateContext()
        {
            var httpContext = this.httpContextAccessor.HttpContext;

            var request = httpContext.Request;
            var user = httpContext.User;
            var runtimeEnvironment = PlatformServices.Default.Runtime;
            var applicationEnvironment = PlatformServices.Default.Application;
            

            // TODO: Fill hostname

            return new AirbrakeContext
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
                UserEmail = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
            };
        }
    }
}