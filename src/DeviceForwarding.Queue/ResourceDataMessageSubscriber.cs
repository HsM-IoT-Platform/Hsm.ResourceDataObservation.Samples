using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Hsm.ResourceData.Messaging.Abstractions.Messages;
using Microsoft.Extensions.Logging;

namespace DeviceForwarding.Queue;

public delegate Task MessageHandler(ResourceDataMessage message, CancellationToken cancellationToken);

public class ResourceDataMessageSubscriber : IAsyncDisposable
{
    private readonly ILogger<ResourceDataMessageSubscriber> _logger;
    private readonly ServiceBusProcessor _processor;

    public ResourceDataMessageSubscriber(
        ILogger<ResourceDataMessageSubscriber> logger,
        ServiceBusProcessor processor)
    {
        _logger = logger;
        _processor = processor;
    }

    private MessageHandler _messageHandler = null!;

    public ValueTask DisposeAsync()
    {
        return this._processor.DisposeAsync();
    }

    public async Task RegisterSubscriberAsync(MessageHandler messageHandler, CancellationToken cancellationToken)
    {
        this._messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        this._processor.ProcessErrorAsync += this.ProcessErrorAsync;
        this._processor.ProcessMessageAsync += this.ProcessMessageAsync;
        await this._processor.StartProcessingAsync(cancellationToken);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs arg)
    {
        var messageAsJson = Encoding.UTF8.GetString(arg.Message.Body.ToArray());
        var message = JsonSerializer.Deserialize<ResourceDataMessage>(messageAsJson, Constants.DefaultSerializerOptions);

        if (message is null)
        {
            this._logger.LogWarning(
                "Received message could not be deserialized to {Type}. Content: {Content}",
                nameof(ResourceDataMessage),
                messageAsJson);

            return;
        }

        await this._messageHandler.Invoke(message, arg.CancellationToken);
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        this._logger.LogError(arg.Exception, "Error while processing message");
        return Task.CompletedTask;
    }
}