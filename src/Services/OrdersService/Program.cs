using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using OrdersService.Services;
using RabbitMQ.Client;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseSqlite("Data Source=./Database/orders.db"));

builder.Services.AddHttpClient<IInventoryClient, InventoryClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["Services:InventoryApi:BaseUrl"]!);
});
builder.Services.AddScoped<IOrdersService, OrdersService.Services.OrdersService>();

var factory = new ConnectionFactory
{
    HostName = builder.Configuration["RabbitMQ:HostName"] ?? "localhost"
};
builder.Services.AddSingleton<IConnection>(factory.CreateConnectionAsync().GetAwaiter().GetResult());
builder.Services.AddSingleton<IOrdersPublisher, OrdersPublisher>();

builder.Services.AddSingleton<IOrderEventsPublisher, KafkaOrderEventsPublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    Directory.CreateDirectory("Database");
    db.Database.EnsureCreated();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
