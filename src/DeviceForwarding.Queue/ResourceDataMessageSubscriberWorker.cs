using System.Text.Json;
using Hsm.ResourceData.Messaging.Abstractions.Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DeviceForwarding.Queue;

public class ResourceDataMessageSubscriberWorker : BackgroundService
{
    private readonly ILogger<ResourceDataMessageSubscriberWorker> _logger;
    private readonly ResourceDataMessageSubscriber _subscriber;

    public ResourceDataMessageSubscriberWorker(
        ILogger<ResourceDataMessageSubscriberWorker> logger,
        ResourceDataMessageSubscriber subscriber)
    {
        _logger = logger;
        _subscriber = subscriber;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this._logger.LogInformation("Starting consumer");
        // since this is capturing a thread
        // there is no need to await it
        // it will run until the host is being stopped
        return _subscriber.RegisterSubscriberAsync(MessageHandler, stoppingToken);
    }

    private Task MessageHandler(ResourceDataMessage message, CancellationToken cancellationToken)
    {
        this._logger.LogInformation(
            "Received message with content: {Content}",
            JsonSerializer.Serialize(message, Constants.WriteIndentedSerializerOptions));

        // here you may hook in your custom logic
        // everytime a new message is received
        // the encompassing function is triggered

        return Task.CompletedTask;
    }
}