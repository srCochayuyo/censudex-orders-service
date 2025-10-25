using DotNetEnv;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.src.Consumers;
using OrderService.src.Data;
using OrderService.src.Interfaces;
using OrderService.src.Messages;
using OrderService.src.Repository;
using Scalar.AspNetCore;


Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var RabbitMQConnectionString = Environment.GetEnvironmentVariable("RabbitMQConnectionString") ?? throw new InvalidOperationException("RabbitMQConnectionString no encontrado.");

builder.Services.AddMassTransit(x =>
{
    // Registrar todos los consumers
    x.AddConsumer<CreateOrderConsumer>();
    x.AddConsumer<StockValidationConsumer>();

    // Configuración de RabbitMQ
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(RabbitMQConnectionString);

        // Cola para el consumer de prueba
        cfg.ReceiveEndpoint("order_products", e =>
        {
            //Consumer de prueba para mensaje que se envia al momento de crear la orden
            e.ConfigureConsumer<CreateOrderConsumer>(context);

        });

        cfg.ReceiveEndpoint("stock_validation", e =>
        {
            e.ConfigureConsumer<StockValidationConsumer>(context);
        });
        

    });
});


string ConnectionString = Environment.GetEnvironmentVariable("OrderConnectionString") ?? throw new InvalidOperationException("OrderConnectionString no encontrado.");
builder.Services.AddDbContext<DBContext>(options => options.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString)));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapControllers(); 
app.Run();
