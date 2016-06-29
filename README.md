# DotNetAirbrake

Airbrake Notifier for ASP.NET MVC 6

AppVeyor 

[![Build status](https://ci.appveyor.com/api/projects/status/ulhdnsd8r4qvge6p?svg=true)](https://ci.appveyor.com/project/ErwinSteffens/dotnetairbrake)

# Why

At my current job I was 'living' between Ruby developers which used Airbrake to monitor application errors. We wanted a single solution for error handling so decided to integrate this in .net MVC 6.

NOTE: We use this with [errbit](https://github.com/errbit/errbit) and have not tested with [Airbrake](https://airbrake.io/) self.

# Todo

- [ ] Add tests!
- [ ] Add hostname to notify message
- [ ] Add body params to notify message
- [ ] Add action and controller context to notify message
- [ ] Add more environment data to notify message

# Usage

## Setup middleware

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

Register services in the service collection and set the options:

```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddAirbrake(options => 
        this.Configuration.GetSection("Airbrake").Bind(options));

    services.AddMvc();
}
```

Add the middleware to the pipeline:

```cs
public void Configure(IApplicationBuilder app)
{
    app.UseAirbrake();

    app.UseMvc(routes =>
    {
        routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
    });
}
```
