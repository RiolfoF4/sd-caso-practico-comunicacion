using Contracts;
using OrdersService.Data;
using OrdersService.Models;

namespace OrdersService.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly OrdersDbContext _ordersDbContext;
        private readonly IInventoryClient _inventory;

        public OrdersService(OrdersDbContext ordersDbContext, IInventoryClient inventory)
        {
            _ordersDbContext = ordersDbContext;
            _inventory = inventory;
        }

        public async Task<OrderResponse?> CreateOrderAsync(OrderRequest request)
        {
            var stock = await _inventory.CheckStockAsync(request.ProductId);
            if (stock is null || !stock.IsAvailable || stock.Stock < request.Quantity)
                return null;

            var order = new Order
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow
            };

            _ordersDbContext.Orders.Add(order);
            await _ordersDbContext.SaveChangesAsync();

            return new OrderResponse(order.Id, order.ProductId, order.Quantity, order.Status, order.CreatedAt);
        }
    }
}
