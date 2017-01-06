using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetAirbrake.AspNetCore
{
    public static class AirbrakeExtensions
    {
        public static IApplicationBuilder UseAirbrake(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<AirbrakeMiddleware>();
        }

        public static IServiceCollection AddAirbrake(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<AirbrakeClient>();

            return services;
        }

        public static IServiceCollection AddAirbrake(this IServiceCollection services, Action<AirbrakeOptions> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            services.Configure(configure);
            return services.AddAirbrake();
        }
    }
}