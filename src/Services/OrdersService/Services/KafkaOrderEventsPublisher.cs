using Confluent.Kafka;
using Contracts;
using System.Text.Json;

namespace OrdersService.Services
{
    public class KafkaOrderEventsPublisher : IOrderEventsPublisher, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaOrderEventsPublisher> _logger;
        private const string Topic = "order-events";

        public KafkaOrderEventsPublisher(
            IConfiguration configuration,
            ILogger<KafkaOrderEventsPublisher> logger)
        {
            _logger = logger;
            var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task PublishAsync(OrderStatusChangedEvent evt)
        {
            var json = JsonSerializer.Serialize(evt);
            var message = new Message<string, string>
            {
                Key = evt.OrderId.ToString(),
                Value = json
            };
            var result = await _producer.ProduceAsync(Topic, message);
            _logger.LogInformation("Evento Kafka publicado: {Status} OrderId={OrderId} en {TopicPartitionOffset}",
                evt.Status, evt.OrderId, result.TopicPartitionOffset);
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}
