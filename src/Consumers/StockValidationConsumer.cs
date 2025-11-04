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
    /// <summary>
    /// Consumidor que procesa mensajes de validación de stock recibidos desde RabbitMQ (a través de MassTransit).
    /// </summary>
    /// <remarks>
    /// Este consumidor gestiona la validación de los ítems de una orden. Mantiene un estado
    /// concurrente por "OrderId" para llevar el conteo de validaciones parciales y
    /// decide cancelar la orden si alguna validación falla o avanzar su estado cuando todas
    /// las validaciones son exitosas.
    /// </remarks>
    public class StockValidationConsumer : IConsumer<StockValidateMessage>
    {
        /// <summary>
        /// Repositorio de Ordenes.
        /// </summary>
        private readonly IOrderRepository _orderRepository;

        /// <summary>
        /// Diccionario concurrente que mantiene el estado de validación (por OrderId) mientras
        /// llegan las respuestas parciales de validacion de stock.
        /// </summary>
        private static readonly ConcurrentDictionary<Guid, ValidationState> _validations = new();
        
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="StockValidationConsumer"/>.
        /// </summary>
        /// <param name="orderRepository">Repositorio de ordenes inyectado para realizar operaciones sobre las ordenes.</param>
        public StockValidationConsumer(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Maneja el mensaje de validacion de stock.
        /// </summary>
        /// <param name="context">Contexto del mensaje provisto por MassTransit que contiene el <see cref="StockValidateMessage"/>.</param>
        /// <returns>Una tarea asincrónica que representa la operación de consumo del mensaje.</returns>
        /// <remarks>
        /// El flujo es el siguiente:
        /// 1. Obtiene el total de items de la orden consultando el repositorio.
        /// 2. Si la validación del mensaje de algun producto es negativa: cancela la orden y limpia el estado.
        /// 3. Si es positiva: incrementa el contador validado para la orden.
        /// 4. Si el contador validado alcanza el total de ítems: cambia el estado de la orden a "En Procesamiento"
        /// y elimina el estado temporal de validación.
        /// </remarks>
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