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


/// <summary>
/// Punto de entrada principal de la aplicación. 
/// Configura los servicios, la base de datos, RabbitMQ y los endpoints gRPC y HTTP.
/// </summary>


Env.Load();

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configura el servidor Kestrel para escuchar en el puerto 5206 con protocolo HTTP/2.
/// </summary>
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5206, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2; 
    });
});

/// <summary>
/// Agrega servicios al contenedor, incluyendo controladores y soporte OpenAPI.
/// </summary>
builder.Services.AddOpenApi();
builder.Services.AddControllers();

/// <summary>
/// Obtiene la cadena de conexion de RabbitMQ desde las variables de entorno.
/// Lanza una excepcion si no se encuentra.
/// </summary>
var RabbitMQConnectionString = Environment.GetEnvironmentVariable("RabbitMQConnectionString") ?? throw new InvalidOperationException("RabbitMQConnectionString no encontrado.");

/// <summary>
/// Registra el sendGrid como un servicio singleton en el contenedor de dependencias.
/// Esto garantiza que se use una unica instancia de sendGrid durante toda la vida util de la aplicacion.
/// </summary>
builder.Services.AddSingleton<SendGridService>();


/// <summary>
/// Configura MassTransit con RabbitMQ y registra el consumidor StockValidationConsumer.
/// </summary>
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StockValidationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(RabbitMQConnectionString);

        cfg.ReceiveEndpoint("stock_validation", e =>
        {
            e.ConfigureConsumer<StockValidationConsumer>(context);
        });
    });
});

/// <summary>
/// Configura la conexion a la base de datos MySQL utilizando la cadena de conexión
/// obtenida desde las variables de entorno.
/// </summary>
string ConnectionString = Environment.GetEnvironmentVariable("OrderConnectionString") ?? throw new InvalidOperationException("OrderConnectionString no encontrado.");
builder.Services.AddDbContext<DBContext>(options => options.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString)));

/// <summary>
/// Registra las dependencias de la aplicacion, incluyendo el repositorio de ordenes.
/// </summary>
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

/// <summary>
/// Agrega soporte para servicios gRPC y su reflexión.
/// </summary>
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

/// <summary>
/// Configura el pipeline de la aplicacion en entorno de desarrollo,
/// habilitando documentación OpenAPI, referencia Scalar y reflexión gRPC.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapGrpcReflectionService();
}

/// <summary>
/// Aplica migraciones pendientes a la base de datos al iniciar la aplicación.
/// </summary>
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DBContext>();
    await context.Database.MigrateAsync();
}

/// <summary>
/// Configura los endpoints HTTPS, controladores y servicios gRPC.
/// </summary>
app.UseHttpsRedirection();
app.MapControllers(); 
app.MapGrpcService<OrderGrpcService>();

/// <summary>
/// Inicia la aplicación.
/// </summary>
app.Run();