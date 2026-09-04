using Confluent.Kafka;
using Contracts;
using System.Text.Json;

namespace NotificationService
{
    public class OrderEventsKafkaWorker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderEventsKafkaWorker> _logger;
        private const string Topic = "order-events";

        public OrderEventsKafkaWorker(
            IConfiguration configuration,
            ILogger<OrderEventsKafkaWorker> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var bootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = "notification-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(Topic);

            _logger.LogInformation("Suscripto a Kafka topic {Topic} con GroupId notification-group ({Bootstrap})", Topic, bootstrapServers);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    if (result is null || result.Message is null)
                    {
                        continue;
                    }

                    var evt = JsonSerializer.Deserialize<OrderStatusChangedEvent>(result.Message.Value);
                    if (evt is null)
                    {
                        continue;
                    }

                    _logger.LogInformation("Kafka: pedido #{OrderId} cambió a {Status} (producto {ProductId}, cant. {Quantity})",
                        evt.OrderId, evt.Status, evt.ProductId, evt.Quantity);

                    _logger.LogInformation("Enviando email por {Status} del pedido #{OrderId}...", evt.Status, evt.OrderId);
                    await Task.Delay(500, stoppingToken);
                    _logger.LogInformation("Email enviado para pedido #{OrderId} [{Status}]", evt.OrderId, evt.Status);

                    consumer.Commit(result);
                }
                catch (ConsumeException ex) when (ex.Error.Code == ErrorCode.UnknownTopicOrPart)
                {
                    _logger.LogInformation("Tópico {Topic} aún sin metadata disponible, reintentando...", Topic);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogWarning("Error de consumo Kafka: {Reason}", ex.Error.Reason);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            consumer.Close();
        }
    }
}
