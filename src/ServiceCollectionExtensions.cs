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

    /// <summary>
    /// Registers an AMQP client with a named configuration and access to the service provider.
    /// This overload allows full access to dependency injection for advanced configuration,
    /// including secrets management, credential protection, or environment-specific logic.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the AMQP client to.</param>
    /// <param name="name">The name of the AMQP client configuration instance.</param>
    /// <param name="configureOptions">
    /// A delegate used to configure the <see cref="ConnectionOptions"/> instance.
    /// The delegate has access to the <see cref="IServiceProvider"/> for resolving dependencies such as logging,
    /// configuration, or custom services (e.g., CredentialService).
    /// </param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is null.</exception>
    /// <remarks>
    /// The registered AMQP client is keyed by name using <c>IKeyedServiceProvider</c>, allowing multiple named clients.
    /// This is useful when connecting to multiple brokers or environments.
    /// </remarks>
    public static IServiceCollection AddAmqpClient(this IServiceCollection services,
                                                   string name,
                                                   Action<ConnectionOptions, IServiceProvider> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<ConnectionOptions>(name);

        services.AddSingleton(sp => new PostConfigureOptions<ConnectionOptions, IServiceProvider>(name, sp, configureOptions));

        services.TryAdd(ServiceDescriptor.KeyedSingleton<IAmqpClient, AmqpClient>(name, (service, name) =>
        {
            var optionsSnapshot = service.GetRequiredService<IOptionsSnapshot<ConnectionOptions>>();

            return new AmqpClient(service.GetRequiredService<ILogger<IAmqpClient>>(),
                                  optionsSnapshot.Get(name?.ToString()));
        }));

        return services;
    }

    /// <summary>
    /// Registers a default AMQP client configuration with access to the service provider.
    /// This overload simplifies registration when only a single client instance is required,
    /// and provides access to dependency injection for advanced configuration scenarios.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the AMQP client to.</param>
    /// <param name="configureOptions">
    /// A delegate used to configure the <see cref="ConnectionOptions"/> instance,
    /// with access to the <see cref="IServiceProvider"/> for resolving additional dependencies.
    /// </param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <remarks>
    /// This is equivalent to calling <c>AddAmqpClient("default", configureOptions)</c>.
    /// </remarks>
    public static IServiceCollection AddAmqpClient(this IServiceCollection services,
                                                   Action<ConnectionOptions, IServiceProvider> configureOptions)
    {
        return services.AddAmqpClient("default", configureOptions);
    }
}