using Microsoft.EntityFrameworkCore;
using InventoryService.Data;
using InventoryService.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlite("Data Source = ./Database/inventory.db"));
builder.Services.AddScoped<IInventoryService, InventoryService.Services.InventoryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    Directory.CreateDirectory("Database");
    db.Database.EnsureCreated();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
