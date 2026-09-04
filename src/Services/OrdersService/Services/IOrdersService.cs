using Contracts;

namespace OrdersService.Services
{
    public interface IOrdersService
    {
        Task<OrderResponse?> CreateOrderAsync(OrderRequest request);
        Task<OrderResponse?> ConfirmOrderAsync(int orderId);
        Task<OrderResponse?> ShipOrderAsync(int orderId);
    }
}
