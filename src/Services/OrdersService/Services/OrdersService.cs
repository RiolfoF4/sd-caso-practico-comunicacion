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
        private readonly IOrderEventsPublisher _events;

        public OrdersService(
            OrdersDbContext ordersDbContext,
            IInventoryClient inventory,
            IOrdersPublisher publisher,
            IOrderEventsPublisher events)
        {
            _ordersDbContext = ordersDbContext;
            _inventory = inventory;
            _publisher = publisher;
            _events = events;
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

            var statusEvent = new OrderStatusChangedEvent(
                order.Id,
                order.ProductId,
                order.Quantity,
                OrderStatus.Created,
                DateTime.UtcNow);
            await _events.PublishAsync(statusEvent);

            return new OrderResponse(order.Id, order.ProductId, order.Quantity, order.Status, order.CreatedAt);
        }

        public async Task<OrderResponse?> ConfirmOrderAsync(int orderId)
        {
            var order = await _ordersDbContext.Orders.FindAsync(orderId);
            if (order is null || order.Status is not OrderStatus.Created)
                return null;

            order.Status = OrderStatus.Confirmed;
            await _ordersDbContext.SaveChangesAsync();

            var evt = new OrderStatusChangedEvent(
                order.Id,
                order.ProductId,
                order.Quantity,
                OrderStatus.Confirmed,
                DateTime.UtcNow);
            await _events.PublishAsync(evt);

            return new OrderResponse(order.Id, order.ProductId, order.Quantity, order.Status, order.CreatedAt);
        }

        public async Task<OrderResponse?> ShipOrderAsync(int orderId)
        {
            var order = await _ordersDbContext.Orders.FindAsync(orderId);
            if (order is null || order.Status is not OrderStatus.Confirmed)
                return null;

            order.Status = OrderStatus.Shipped;
            await _ordersDbContext.SaveChangesAsync();

            var evt = new OrderStatusChangedEvent(
                order.Id,
                order.ProductId,
                order.Quantity,
                OrderStatus.Shipped,
                DateTime.UtcNow);
            await _events.PublishAsync(evt);

            return new OrderResponse(order.Id, order.ProductId, order.Quantity, order.Status, order.CreatedAt);
        }
    }
}
