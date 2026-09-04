using AnalyticsService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<OrderEventsAnalyticsWorker>();

var host = builder.Build();
host.Run();
