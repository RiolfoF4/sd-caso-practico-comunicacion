using Contracts;

namespace OrdersService.Services
{
    public interface IOrdersService
    {
        public Task<OrderResponse?> CreateOrderAsync(OrderRequest request);
    }
}
