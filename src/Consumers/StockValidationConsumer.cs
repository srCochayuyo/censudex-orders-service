using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using OrderService.src.Dto;
using OrderService.src.Interfaces;
using OrderService.src.Messages;

namespace OrderService.src.Consumers
{
    public class StockValidationConsumer : IConsumer<StockValidateMessage>
    {
        private readonly IOrderRepository _orderRepository;

        private static readonly ConcurrentDictionary<Guid, ValidationState> _validations = new();
        
        public StockValidationConsumer(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Consume(ConsumeContext<StockValidateMessage> context)
        {

            var message = context.Message;
            Console.WriteLine($"[RabbitMQ] Validacio de stock para {message.OrderId}");

            var totalItems = await _orderRepository.CountItemsOrderById(message.OrderId);

            var state = _validations.GetOrAdd(message.OrderId, new ValidationState { Total = totalItems });

            if (!message.ValidationResult)
            {
                await _orderRepository.CancelateOrder(message.OrderId, null);
                Console.WriteLine($"[RabbitMQ] Orden cancelada por stock insuficiente");
                _validations.TryRemove(message.OrderId, out _);
                return;
            }

            var validated = Interlocked.Increment(ref state.ValidatedCount);

            Console.WriteLine($"[RabbitMQ] Progreso: {validated}/{totalItems}");

            if (validated == totalItems)
            {

                var stateDto = new ChangeStateDto
                {
                    OrderStatus = "En Procesamiento",
                };

                await _orderRepository.ChangeStateOrder(message.OrderId, null, stateDto);
                Console.WriteLine($"[RabbitMQ] Validacion de stock exitosa");
                _validations.TryRemove(message.OrderId, out _);

            }
        }
        
        private class ValidationState
        {
            public int Total;
            public int ValidatedCount;
        }


    }
}