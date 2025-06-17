namespace SalesService.Bus
{
    public interface IMessagePublisher
    {
        Task PublishAsync(string messageType, string payload);
    }
}