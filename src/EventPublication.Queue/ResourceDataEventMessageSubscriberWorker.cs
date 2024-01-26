using System.Text.Json;
using Hsm.ResourceData.Messaging.Abstractions.Messages;
using Hsm.ResourceDataObservation.Messaging.Abstractions.EventPublication.Models;
using Hsm.ResourceDataObservation.Messaging.Abstractions.Subscriber;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventPublication.Queue;

public class ResourceDataEventMessageSubscriberWorker : BackgroundService
{
    private readonly ILogger<ResourceDataEventMessageSubscriberWorker> _logger;
    private readonly IResourceDataEventMessageSubscriber _subscriber;

    public ResourceDataEventMessageSubscriberWorker(
        ILogger<ResourceDataEventMessageSubscriberWorker> logger,
        IResourceDataEventMessageSubscriber subscriber)
    {
        _logger = logger;
        _subscriber = subscriber;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this._logger.LogInformation("Starting subscriber");
        // since this is capturing a thread
        // there is no need to await it
        // it will run until the host is being stopped
        return _subscriber.RegisterSubscriberAsync(MessageHandler, stoppingToken);
    }

    private Task MessageHandler(ResourceDataEventMessage message, CancellationToken cancellationToken)
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