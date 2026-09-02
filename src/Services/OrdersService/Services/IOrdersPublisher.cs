using Contracts;

namespace OrdersService.Services
{
    public interface IOrdersPublisher
    {
        Task PublishOrderCreatedAsync(OrderCreatedEvent orderCreatedEvent);
    }
}
