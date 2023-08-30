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
}

