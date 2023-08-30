using System;
using Consul;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace ProductApi.BackgrounServices;

public class ServiceDiscoveryHostedService : IHostedService
{
    private readonly IServer _server;
    private readonly IConsulClient _client;

    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    private AgentServiceRegistration _registration;

    public ServiceDiscoveryHostedService(IServer server, IConsulClient client, IHostApplicationLifetime hostApplicationLifetime)
    {
        _server = server;
        _client = client;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Addresses before application has started: {GetAddresses()}");

        _hostApplicationLifetime.ApplicationStarted.Register(async () =>
        {
            Console.WriteLine($"Addresses after application has started: {GetAddresses()}");

            var address = _server.Features.Get<IServerAddressesFeature>().Addresses.First();
            var uri = new Uri(address);

            _registration = new AgentServiceRegistration
            {
                ID = "Product-" + uri.Port,
                Name = "product",
                Address = $"{uri.Host}",
                Port = uri.Port,
                Tags = new[] {"Product"},
                Check = new AgentServiceCheck
                {
                    //HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}/health",
                    HTTP = $"{uri.Scheme}://host.docker.internal:{uri.Port}/health", //for docker
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)
                }

            };
            
            // Deregister already registered service
            await _client.Agent.ServiceDeregister(_registration.ID, cancellationToken).ConfigureAwait(false);

            // Registers service
            await _client.Agent.ServiceRegister(_registration, cancellationToken).ConfigureAwait(false);
        });

        
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_registration != null)
        {
            await _client.Agent.ServiceDeregister(_registration.ID, cancellationToken).ConfigureAwait(false);
        }        
    }

    private string GetAddresses()
    {
        var addresses = _server.Features.Get<IServerAddressesFeature>().Addresses;
        return string.Join(", ", addresses);
    }

}

