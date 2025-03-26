using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Korjn.AmqpClientInject.DependencyInjection;
public static class ServiceCollectionExtensions
{
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

    public static IServiceCollection AddAmqpClient(this IServiceCollection services,
                                                   Action<ConnectionOptions> configureOptions)
    {
        return services.AddAmqpClient("default", configureOptions);
    }
}