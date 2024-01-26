using EventPublication.Queue;
using Hsm.ResourceDataObservation.Messaging.ServiceBus.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var host = Host.CreateDefaultBuilder();

host.UseSerilog((context, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(context.Configuration));

host.ConfigureServices((context, services) =>
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

    services.AddResourceDataEventMessageSubscriber(
        ruleQueueConfiguration.QueueConnectionString,
        ruleQueueConfiguration.QueueName);

    services.AddHostedService<ResourceDataEventMessageSubscriberWorker>();
});

await host.RunConsoleAsync();