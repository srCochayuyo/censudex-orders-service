using DotNetEnv;
using MassTransit;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using OrderService.src.Consumers;
using OrderService.src.Data;
using OrderService.src.Interfaces;
using OrderService.src.Messages;
using OrderService.src.Repository;
using OrderService.src.Service;
using Scalar.AspNetCore;


Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    options.ListenAnyIP(int.Parse(port));

    options.ListenAnyIP(int.Parse(port), listenOptions =>
    {
        
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
    
});

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
            //Consumer de prueba para verficiar que se envio el mensaje
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

builder.Services.AddGrpc();

builder.Services.AddGrpcReflection();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapGrpcReflectionService();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DBContext>();
    await context.Database.MigrateAsync();

}

app.UseHttpsRedirection();
app.MapControllers(); 
app.MapGrpcService<OrderGrpcService>();
app.Run();
