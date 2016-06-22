using System;
using DotNetAirBrake.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace DotNetAirBrake
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

            services.AddSingleton<IAirbrakeClient, AirbrakeClient>();
            services.AddSingleton<IAirbrakeMessageBuilder, AirbrakeMessageBuilder>();

            // Add all notice part builders in the assembly
            services.Scan(s => s.FromAssemblyOf<INoticeMessageBuilder>()
                                .AddClasses(c => c.AssignableTo<INoticeMessageBuilder>())
                                .AsMatchingInterface()
                                .WithSingletonLifetime());

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