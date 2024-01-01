namespace ServiceBusDemo.Abstraction.Models;

public class Message<T>
{
    public string Title { get; set; } = null!;
    
    public T Data { get; set; } = default!;
}