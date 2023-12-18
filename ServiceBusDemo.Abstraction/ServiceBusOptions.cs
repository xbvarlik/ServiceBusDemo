namespace ServiceBusDemo.Abstraction;

public class ServiceBusOptions
{
    public string ConnectionString { get; set; }
    public List<string> Queues { get; set; } = new List<string>();
}