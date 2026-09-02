using NotificationService;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

var factory = new ConnectionFactory
{
    HostName = builder.Configuration["RabbitMQ:HostName"] ?? "localhost"
};
builder.Services.AddSingleton<IConnection>(factory.CreateConnectionAsync().GetAwaiter().GetResult());
builder.Services.AddHostedService<OrderCreatedWorker>();

var host = builder.Build();
host.Run();
