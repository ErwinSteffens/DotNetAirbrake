using System;
using DotNetAirbrake;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsoleAppSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true);
            var config = builder.Build();

            var airbrakeOptions = new AirbrakeOptions();
            config.GetSection("Airbrake").Bind(airbrakeOptions);

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(config.GetSection("Logging"));
            loggerFactory.AddDebug();

            var client = new AirbrakeClient(loggerFactory, airbrakeOptions);

            Console.WriteLine("Sending the exception now!");

            try
            {
                Program.RaiseException();
            }
            catch (Exception exc)
            {
                exc.SendToAirbrakeAsync(client).Wait();

                Console.WriteLine("Exception has been send!");
            }
        }

        public static void RaiseException()
        {
            throw new InvalidOperationException("Test exception message");
        }
    }
}