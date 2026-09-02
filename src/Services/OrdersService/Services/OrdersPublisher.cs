using Contracts;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace OrdersService.Services
{
    public class OrdersPublisher : IOrdersPublisher
    {
        private readonly IConnection _connection;
        private const string QueueName = "orders_created_queue";

        public OrdersPublisher(IConnection connection)
        {
            _connection = connection;
        }

        public async Task PublishOrderCreatedAsync(OrderCreatedEvent orderCreatedEvent)
        {
            using var channel = await _connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );

            var json = JsonSerializer.Serialize(orderCreatedEvent);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: QueueName,
                mandatory: false,
                basicProperties: properties,
                body: body);
        }
    }
}
