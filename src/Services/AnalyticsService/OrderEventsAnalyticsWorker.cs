using Confluent.Kafka;
using Contracts;
using System.Text.Json;

namespace AnalyticsService
{
    public class OrderEventsAnalyticsWorker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderEventsAnalyticsWorker> _logger;
        private readonly Dictionary<OrderStatus, int> _counts = [];
        private const string Topic = "order-events";

        public OrderEventsAnalyticsWorker(
            IConfiguration configuration,
            ILogger<OrderEventsAnalyticsWorker> logger)
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
                GroupId = "analytics-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(Topic);

            _logger.LogInformation("Analytics suscripto a {Topic} con GroupId analytics-group ({Bootstrap})", Topic, bootstrapServers);

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

                    if (_counts.ContainsKey(evt.Status))
                    {
                        _counts[evt.Status]++;
                    }
                    else
                    {
                        _counts[evt.Status] = 1;
                    }

                    _logger.LogInformation("Analytics: pedido #{OrderId} [{Status}] | totales Created={Created} Confirmed={Confirmed} Shipped={Shipped}",
                        evt.OrderId, evt.Status,
                        _counts.GetValueOrDefault(OrderStatus.Created),
                        _counts.GetValueOrDefault(OrderStatus.Confirmed),
                        _counts.GetValueOrDefault(OrderStatus.Shipped));

                    consumer.Commit(result);
                }
                catch (ConsumeException ex) when (ex.Error.Code == ErrorCode.UnknownTopicOrPart)
                {
                    _logger.LogInformation("Tópico {Topic} aún sin metadata disponible, reintentando...", Topic);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogWarning("Error de consumo Kafka (analytics): {Reason}", ex.Error.Reason);
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
