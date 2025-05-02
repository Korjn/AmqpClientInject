using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Amqp;
using Amqp.Framing;
using Amqp.Sasl;
using Microsoft.Extensions.Options;

namespace Korjn.AmqpClientInject;

internal class AmqpClient(ILogger<IAmqpClient> logger, IOptions<AmqpConnectionOptions> _options) : IAmqpClient, IDisposable
{
    private readonly AmqpConnectionOptions options = _options.Value;
    private Connection? connection;
    private Session? session;
    private bool disposedValue;
    private readonly ConcurrentDictionary<string, SenderLink> senderLinks = new();
    private readonly SemaphoreSlim semaphore = new(1, 1); // Защита от гонки подключений    

    private string Url => $"amqp://{options?.Host}:{options?.Port}";

    private void ConnectionClosed(IAmqpObject sender, Error error)
    {
        senderLinks.Clear();
        session = null;
        connection = null;

        logger.LogError("Connection with {url} closed. Error: {error}", Url, error?.ToString() ?? sender.Error?.ToString() ?? "Unknown reason");
    }

    private void OnSenderLinkClosed(IAmqpObject sender, Error error)
    {
        _ = senderLinks?.TryRemove((sender as Link)?.Name ?? string.Empty, out _);
    }

    private bool IsConnectionClosed() => connection?.IsClosed ?? true;

    private void OpenSession()
    {
        try
        {
            logger.LogTrace("Attempting open session with {url}", Url);

            session = new Session(connection);

            logger.LogTrace("Session successfully establishe with {url}", Url);
        }
        catch (Exception e)
        {
            logger.LogError("Open session error with {url} =>{message}", Url, e.Message);
            session = null;
            connection?.Close();
            throw;
        }
    }

    private async Task OpenConnectionAsync(Address address, Open open)
    {
        if (IsConnectionClosed())
            try
            {
                logger.LogTrace("Attempting connection with {url}", Url);

                Connection.Factory.SASL.Profile = SaslProfile.Anonymous;

                connection = await Connection.Factory.CreateAsync(address, open);

                connection.AddClosedCallback(ConnectionClosed);

                logger.LogTrace("Connection successfully establishe with {url}", Url);

                OpenSession();
            }
            catch (Exception e)
            {
                logger.LogError("Connection error with {url} =>{message}", Url, e.Message);
                connection = null;
                throw;
            }
    }


    private async Task EnsureConnectedAsync()
    {
        if (!IsConnectionClosed())
        {
            return;
        }

        await semaphore.WaitAsync();

        try
        {

            var address = new Address(host: options?.Host,
                                              port: options?.Port ?? 0,
                                              user: options?.UserName,
                                              password: options?.Password,
                                              scheme: "AMQP");

            var open = new Open()
            {
                // The application must ensure that the mandatory field, ContainerId, is set to avoid connection failures.
                ContainerId = $"{Environment.MachineName}-[{options?.ApplicationName}]",

                // Устанавливаем максимальный размер фрейма 64 КБ, это часто рекомендуемый размер для брокеров AMQP
                MaxFrameSize = 1024 * 64
            };

            try
            {
                await OpenConnectionAsync(address, open);
            }
            catch
            {
                int delay = 2000; // Начальная задержка в 2 секунды

                while (true)
                {
                    try
                    {
                        await OpenConnectionAsync(address, open);                        
                        break;
                    }
                    catch
                    {
                        logger.LogTrace("Retrying connection with {url} in {delay} sec...", Url, delay / 1000);
                        await Task.Delay(delay);
                        delay = Math.Min(delay + 1000, 40000); // Увеличение задержки до 40 сек
                    }
                }
            }
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task<SenderLink> CreateSenderAsync(string name, string address)
    {
        await EnsureConnectedAsync();

        return senderLinks.GetOrAdd(name, key =>
        {
            logger.LogTrace("Attempting to create sender for {url}/{address}", Url, address);

            try
            {
                var newSender = new SenderLink(session, key, address);
                newSender.AddClosedCallback(OnSenderLinkClosed);

                logger.LogTrace("Sender to {url}/{address} successfully created", Url, address);

                return newSender;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error creating sender to {url}/{address}", Url, address);
                throw;
            }
        });
    }

    public async Task<ReceiverLink> CreateReceiverAsync(string name, string address)
    {
        await EnsureConnectedAsync();

        logger.LogTrace("Attempting create receiver for {url}/{address}", Url, address);

        try
        {
            var result = new ReceiverLink(session, name, address);

            logger.LogTrace("Receiver to {url}/{address} successfully created", Url, address);

            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating receiver to {url}/{address}", Url, address);
            throw;
        }
    }    

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: освободить управляемое состояние (управляемые объекты)

                try
                {
                    connection?.Close();
                }
                catch (Exception)
                {
                }
            }

            // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
            // TODO: установить значение NULL для больших полей
            disposedValue = true;
        }
    }

    // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
    // ~AmqpConnection()
    // {
    //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}