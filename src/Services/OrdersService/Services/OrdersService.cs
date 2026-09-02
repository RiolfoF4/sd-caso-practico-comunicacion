using Contracts;
using OrdersService.Data;
using OrdersService.Models;

namespace OrdersService.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly OrdersDbContext _ordersDbContext;
        private readonly IInventoryClient _inventory;
        private readonly IOrdersPublisher _publisher;

        public OrdersService(
            OrdersDbContext ordersDbContext,
            IInventoryClient inventory,
            IOrdersPublisher publisher)
        {
            _ordersDbContext = ordersDbContext;
            _inventory = inventory;
            _publisher = publisher;
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

            await _inventory.DeductStockAsync(request.ProductId, request.Quantity);

            var orderCreatedEvent = new OrderCreatedEvent(
                order.Id,
                order.ProductId,
                order.Quantity,
                order.CreatedAt);
            await _publisher.PublishOrderCreatedAsync(orderCreatedEvent);

            return new OrderResponse(order.Id, order.ProductId, order.Quantity, order.Status, order.CreatedAt);
        }
    }
}
