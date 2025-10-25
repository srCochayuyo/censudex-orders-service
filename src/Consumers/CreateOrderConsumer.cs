using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using OrderService.src.Messages;

namespace OrderService.src.Consumers
{
    public class CreateOrderConsumer : IConsumer<SenderMessage>
    {
        public Task Consume(ConsumeContext<SenderMessage> context)
        {
            Console.WriteLine("Mensaje recibido:");
            Console.WriteLine($"OrderId: {context.Message.OrderId}");
            foreach (var p in context.Message.Items)
            {
                Console.WriteLine($"ProductId: {p.ProductId}, Quantity: {p.Quantity}");
            }
            return Task.CompletedTask;
        }
    }
}