using DotNetAirbrake;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MvcAppSample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Load configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add Airbrake options from configuration
            services.AddAirbrake(options => 
                this.Configuration.GetSection("AirBrake").Bind(options));

            // Add Airbrake options from code
            services.AddAirbrake(options =>
            {
                options.ProjectId = "my-airbrake-project-id";
                options.ProjectKey = "my-airbrake-project-key";
                options.Url = "https://airbrake.io";
            });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
