using Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace NotificationService
{
    public class OrderCreatedWorker : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly ILogger<OrderCreatedWorker> _logger;
        private const string QueueName = "orders_created_queue";

        public OrderCreatedWorker(IConnection connection, ILogger<OrderCreatedWorker> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var channel = await _connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(json);

                _logger.LogInformation("Pedido recibido: OrderId={OrderId}, Producto={ProductId}, Cantidad={Quantity}",
                    evt.OrderId, evt.ProductId, evt.Quantity);

                _logger.LogInformation("Enviando email de confirmación...");
                await Task.Delay(1000);
                _logger.LogInformation("Email enviado exitosamente para pedido #{OrderId}", evt.OrderId);

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
