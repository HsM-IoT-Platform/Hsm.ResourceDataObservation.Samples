using Azure.Messaging.ServiceBus;
using DeviceForwarding.Queue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

var host = Host.CreateDefaultBuilder();

host.UseSerilog((context, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(context.Configuration));

host.ConfigureServices((context, services) =>
{
    services.AddSingleton<ResourceDataMessageConsumer>(provider =>
    {
        var ruleQueueConfiguration = context
            .Configuration
            .GetSection(nameof(RuleQueueConfiguration))
            .Get<RuleQueueConfiguration>();

        if (ruleQueueConfiguration is null)
        {
            throw new InvalidOperationException(
                $"{nameof(RuleQueueConfiguration)} could not be loaded from app-configuration!");
        }

        var serviceBusClient = new ServiceBusClient(ruleQueueConfiguration.QueueConnectionString);
        var serviceBusProcessor = serviceBusClient.CreateProcessor(ruleQueueConfiguration.QueueName);

        return new ResourceDataMessageConsumer(
            provider.GetRequiredService<ILogger<ResourceDataMessageConsumer>>(),
            serviceBusProcessor);
    });

    services.AddHostedService<ResourceDataMessageConsumerWorker>();
});

await host.RunConsoleAsync();