using Microsoft.Extensions.DependencyInjection;

namespace Korjn.AmqpClientInject.DependencyInjection;

/// <summary>
/// Provides extension methods for registering AMQP client services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers an AMQP client service in the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the AMQP client to.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="AmqpConnectionOptions"/> instance.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <c>null</c>.</exception>
    public static IServiceCollection AddAmqpClient(this IServiceCollection services,
                                                   Action<AmqpConnectionOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<AmqpConnectionOptions>().Configure(configureOptions);
        services.AddSingleton<IAmqpClient, AmqpClient>();

        return services;
    }

    /// <summary>
    /// Registers an AMQP client service with access to the <see cref="IServiceProvider"/> during configuration.
    /// This overload allows resolving other services or configuration values from the dependency injection container
    /// when setting up the <see cref="AmqpConnectionOptions"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the AMQP client to.</param>
    /// <param name="configureOptions">
    /// A delegate that configures the <see cref="AmqpConnectionOptions"/> instance and has access to the <see cref="IServiceProvider"/>.
    /// </param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <c>null</c>.</exception>
    public static IServiceCollection AddAmqpClient(this IServiceCollection services,
                                                   Action<AmqpConnectionOptions, IServiceProvider> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<AmqpConnectionOptions>().Configure(configureOptions);
        services.AddSingleton<IAmqpClient, AmqpClient>();

        return services;
    }
}