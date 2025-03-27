using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Korjn.AmqpClientInject.DependencyInjection;
/// <summary>
/// Provides extension methods for registering AMQP client services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers an AMQP client with a named configuration.
    /// </summary>
    /// <param name="services">The service collection to add the AMQP client to.</param>
    /// <param name="name">The name of the AMQP client configuration.</param>
    /// <param name="configureOptions">The configuration options for the AMQP connection.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddAmqpClient(this IServiceCollection services,
                                                   string name,
                                                   Action<ConnectionOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Configure(name, configureOptions);

        services.TryAdd(ServiceDescriptor.KeyedSingleton<IAmqpClient, AmqpClient>(name, (service, name) =>
        {
            var optionsSnapshot = service.GetRequiredService<IOptionsSnapshot<ConnectionOptions>>();

            return new AmqpClient(service.GetRequiredService<ILogger<IAmqpClient>>(),
                                  optionsSnapshot.Get(name?.ToString()));
        }));

        return services;
    }

    /// <summary>
    /// Registers an AMQP client with a default configuration.
    /// </summary>
    /// <param name="services">The service collection to add the AMQP client to.</param>
    /// <param name="configureOptions">The configuration options for the AMQP connection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAmqpClient(this IServiceCollection services,
                                                   Action<ConnectionOptions> configureOptions)
    {
        return services.AddAmqpClient("default", configureOptions);
    }
}