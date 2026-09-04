using Contracts;

namespace OrdersService.Services
{
    public interface IOrderEventsPublisher
    {
        Task PublishAsync(OrderStatusChangedEvent evt);
    }
}
