using NotificationService;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

var factory = new ConnectionFactory
{
    HostName = builder.Configuration["RabbitMQ:HostName"] ?? "localhost"
};
builder.Services.AddSingleton<IConnection>(factory.CreateConnectionAsync().GetAwaiter().GetResult());
builder.Services.AddHostedService<OrderCreatedWorker>();

builder.Services.AddHostedService<OrderEventsKafkaWorker>();

var host = builder.Build();
host.Run();
