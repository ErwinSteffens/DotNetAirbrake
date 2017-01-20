# DotNetAirbrake

Airbrake Notifier for ASP.NET MVC 6

AppVeyor:

[![Build status](https://ci.appveyor.com/api/projects/status/ulhdnsd8r4qvge6p?svg=true)](https://ci.appveyor.com/project/ErwinSteffens/dotnetairbrake)

# Why

At my current job I was 'living' between Ruby developers which used Airbrake to monitor application errors. We wanted a single solution for error handling so decided to integrate this in .NET MVC 6.

NOTE: We use this with [errbit](https://github.com/errbit/errbit) and have not tested with [Airbrake](https://airbrake.io/) self.

# Todo

- [ ] Add tests!
- [ ] Add hostname to notify message
- [ ] Add body params to notify message
- [ ] Add action and controller context to notify message
- [ ] Add more environment data to notify message

# Usage

## Configuration

Add a configuration section in your `appsettings.json`:

```json
{
  "Airbrake": {
    "Url": "https://airbrake.io/",
    "ProjectId": "my-project-id",
    "ProjectKey": "my-project-key"
  }
}
```

## ASP.NET middleware

Install ASP.NET Core package:

```PowerShell
Install-Package DotNetAirbrake.AspNetCore
```

Register services in the service collection and set the options:

```cs
public void ConfigureServices(IServiceCollection services)
{
    // Add Airbrake services
    services.AddAirbrake(options => 
        this.Configuration.GetSection("Airbrake").Bind(options));

    // Add Mcv services
    services.AddMvc();
}
```

Add the middleware to the pipeline. When an exception is raised in any of the middlewares higher in the pipeline, the exception is automatticly send to your Airbrake service.

```cs
public void Configure(IApplicationBuilder app)
{
    // Add Airbrake middleware
    app.UseAirbrake();

    // Add Mvc middelware
    app.UseMvc();
}
```

## Send exception manually

Install client only package:

```PowerShell
Install-Package DotNetAirbrake
```

Create the Airbrake client:

```cs
// Create a configuration
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true, true)
    .Build();

// Load airbrake options
var airbrakeOptions = new AirbrakeOptions();
config.GetSection("Airbrake").Bind(airbrakeOptions);

// Create logger factory
var loggerFactory = new LoggerFactory();
loggerFactory.AddConsole(config.GetSection("Logging"));

// Create the client
var client = new AirbrakeClient(loggerFactory, airbrakeOptions);
```

Send your exceptions to your Airbrake service:

```cs
try
{
    throw new InvalidOperationException("Test exception");
}
catch (Exception exc)
{
    // Send the exception to airbrake
    await exc.SendToAirbrakeAsync(client);
}
```