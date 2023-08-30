using System;
using Consul;

namespace ProductApi.Extensions;

public static class ConsulExtensions
{
	public static IServiceCollection AddConsulConfig(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddSingleton<IConsulClient, ConsulClient>(x =>
		{
			return new ConsulClient(_consulClientConfig =>
			{
				var address = configuration.GetValue<string>("Consul:Host");
				_consulClientConfig.Address = new Uri(address);
			});
		});

		return services;
	}

	public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
	{
		var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
        var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("AppExtensions");
        var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

        if (!(app.Properties["server.Features"] is Microsoft.AspNetCore.Http.Features.FeatureCollection features))
			return app;

		var addresses = features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>();
		var address = addresses.Addresses.First();

        Console.WriteLine($"address={address}");

        var uri = new Uri(address);
		var registration = new AgentServiceRegistration()
		{
			ID = "Product-" + uri.Port,
			Name = "product",
			Address = $"{uri.Scheme}://{uri.Host}",
			Port = uri.Port,
			Tags = new[] {""},
			Check = new AgentServiceCheck
			{
				HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}/health",
				Interval = TimeSpan.FromSeconds(10),
				Timeout = TimeSpan.FromSeconds(5),
				DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)
			}
		};

        logger.LogInformation("Registering with Consul");
		consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        consulClient.Agent.ServiceRegister(registration).Wait();

		lifetime.ApplicationStopping.Register(() =>
		{
            logger.LogInformation("Unregistering from Consul");
			consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        });

        return app;
	}


}

