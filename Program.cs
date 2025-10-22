using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using OrderService.src.Data;
using OrderService.src.Interfaces;
using OrderService.src.Repository;
using Scalar.AspNetCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();


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
