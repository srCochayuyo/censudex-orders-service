using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.src.Data;
using OrderService.src.Dto;
using OrderService.src.Interfaces;
using OrderService.src.Mappers;
using OrderService.src.Models;

namespace OrderService.src.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DBContext _context;

        public OrderRepository(DBContext context)
        {
            _context = context;
        }


        //POST Crear orden
        public async Task<ResponseOrderDto> CreateOrder(CreateOrderDto request)
        {
            var OrderRequest = new Order
            {
                UserId = request.UserId,
                OrderNumber = CreateOrderNumber(),
                Address = request.Address,
                OrderStatus = "Pendiente",
                CreateAt = DateOnly.FromDateTime(DateTime.UtcNow),
                Items = request.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            //TODO: Publicar evento en RabbitMQ

            await _context.Orders.AddAsync(OrderRequest);
            await _context.OrderItems.AddRangeAsync(OrderRequest.Items);

            await _context.SaveChangesAsync();

            return OrderRequest.ToOrderResponse();
        }


        //GET obtener order por id o por numero de pedido
        public async Task<ResponseOrderDto?> GetOrderByIdentifier(Guid? OrderId, string? OrderNumber)
        {
            var order = await GetOrderByIdOrOrderNumber(OrderId, OrderNumber,true);

            return order?.ToOrderResponse();

        }




        //TODO: PUT actualizar estado de un pedido (ADMIN)
        public async Task<ResponseChangeStateDto> ChangeStateOrder(Guid? OrderId, string? OrderNumber, ChangeStateDto request)
        {

            var order = await GetOrderByIdOrOrderNumber(OrderId, OrderNumber);

            if (order == null)
            {
                throw new Exception("Error: Pedido no encontrado");
            }

            if (string.IsNullOrWhiteSpace(request.TrackingNumber) && request.OrderStatus.ToLower() == "enviado")
            {
                throw new Exception("Error: El numero de seguimiento es requerido para cambiar el estado a Enviado");
            }

            if (!string.IsNullOrWhiteSpace(request.TrackingNumber) && request.OrderStatus.ToLower() == "enviado")
            {
                throw new Exception("Error: El número de seguimiento solo puede asignarse cuando el estado es Enviado.");
            }

            if (!string.IsNullOrWhiteSpace(request.TrackingNumber))
            {
                order.TrackingNumber = request.TrackingNumber;
            }

            order.OrderStatus = request.OrderStatus;
            order.UpdateAt = DateOnly.FromDateTime(DateTime.UtcNow);

            await _context.SaveChangesAsync();

            return order.ToChangeStateResponse();;


        }

        //PUT cancelar pedido cambiando el estado del mismo
        public async Task<ResponseChangeStateDto> CancelateOrder(Guid? OrderId, string? OrderNumber)
        {
            var order = await GetOrderByIdOrOrderNumber(OrderId, OrderNumber);

            if (order == null)
            {
                throw new Exception("Error: Pedido no encontrado");
            }

            if (order.OrderStatus == "Cancelado")
            {
                throw new Exception("Error: Pedido ya cancelado");
            }

            if(order.OrderStatus == "Enviado")
            {
                throw new Exception("Error: No es posible cancelar el pedido porque ya fue enviado.");
            }

            order.OrderStatus = "Cancelado";
            order.UpdateAt = DateOnly.FromDateTime(DateTime.UtcNow);

            await _context.SaveChangesAsync();

            return order.ToChangeStateResponse();;

        }
        

        //TODO: GET Obtener Historia historico de pedidos de un cliente, Filtros por ID o numero de pedido, por rango de fecha de cracion

        //TODO: GET obtener historicos de clientes con filtros id o numero de pedido, rango de fechas de cracion, id o nombre de cliente (ADMIN)

        //Funcion para obtener ordern con identificador (pueder ser ID o Numero de Orden)
        private async Task<Order?> GetOrderByIdOrOrderNumber(Guid? OrderId, string? OrderNumber, bool includeItems = false)
        {
            IQueryable<Order> query = _context.Orders;

            if (includeItems)
            {
                query = query.Include(o => o.Items);
            }

            if (OrderId != null)
            {
                return await query.FirstOrDefaultAsync(o => o.Id == OrderId.Value);
            }

            if (!string.IsNullOrWhiteSpace(OrderNumber))
            {
                return await query.FirstOrDefaultAsync(o => o.OrderNumber == OrderNumber);
            }

            return null;
        }
        
        //Funcion para crar numero de pedido aleatorio sin repetirse
        private string CreateOrderNumber()
        {

            string orderNumber = "";

            Random random = new Random();

            while (true)
            {

                int numberPart = random.Next(1000, 10000);

                orderNumber = $"CEN-{numberPart}";

                var exist = _context.Orders.Any(o => o.OrderNumber == orderNumber);

                if (!exist)
                {
                    return orderNumber;
                }

            }
        }
    }
}