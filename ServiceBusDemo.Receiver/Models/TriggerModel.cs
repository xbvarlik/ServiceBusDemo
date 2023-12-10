namespace ServiceBusDemo.Receiver.Models;

public class TriggerModel
{
    public int Id { get; set; }

    public string Message { get; set; } = null!;
}