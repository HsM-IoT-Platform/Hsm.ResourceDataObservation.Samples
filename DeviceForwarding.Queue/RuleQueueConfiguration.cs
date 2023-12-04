namespace DeviceForwarding.Queue;

public class RuleQueueConfiguration
{
    public string QueueName { get; set; } = null!;

    public string QueueConnectionString { get; set; } = null!;
}