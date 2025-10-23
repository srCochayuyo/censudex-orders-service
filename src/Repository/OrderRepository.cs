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

            var response = OrderRequest.ToOrderResponse();
            return response;
        }


        //TODO: GET obtener order por id o por numero de pedido
        public async Task<ResponseOrderDto?> GetOrderByIdorOrderNumber(Guid? OrderId, string? OrderNumber)
        {
            var query = _context.Orders.Include(o => o.Items).AsQueryable();

            Order? order = null;

            if (OrderId != null)
            {
                order = await query.FirstOrDefaultAsync(o => o.Id == OrderId);
            }

            if (OrderNumber != null)
            {
                order = await query.FirstOrDefaultAsync(o => o.OrderNumber == OrderNumber);

            }

            return order?.ToOrderResponse();

        }




        //TODO: PUT actualizar estado de un pedido (ADMIN)
        public async Task<ResponseChangeStateDto> ChangeStateOrder(Guid? OrderId, string? OrderNumber,ChangeStateDto request)
        {

            Order? order = null;

            if (OrderId != null)
            {
                order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == OrderId);
            }

            if ( OrderNumber != null)
            {
                order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderNumber == OrderNumber);

            }

            if (order == null)
            {
                throw new Exception("Error: Pedido no encontrado");
            }

            if (string.IsNullOrWhiteSpace(request.TrackingNumber) && request.OrderStatus.ToLower() == "enviado")
            {
                throw new Exception("Error: El numero de seguimiento es requerido para cambiar el estado a Enviado");
            }
            
            if(!string.IsNullOrWhiteSpace(request.TrackingNumber) && request.OrderStatus.ToLower() == "enviado" )
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

            var response = order.ToChangeStateResponse();

            return response;

           
        }

        //TODO: PUT cancelar pedido cambiando el estado del mismo

        //TODO: GET Obtener Historia historico de pedidos de un cliente, Filtros por ID o numero de pedido, por rango de fecha de cracion
        
        //TODO: GET obtener historicos de clientes con filtros id o numero de pedido, rango de fechas de cracion, id o nombre de cliente (ADMIN)

        //Funcion para crar numero de pedido aleatorio sin repetirse
        public string CreateOrderNumber()
        {

            string orderNumber = "";

            Random random = new Random();

            while (true)
            {

                int numberPart = random.Next(1000, 10000);

                orderNumber = $"CEN-{numberPart}";

                var exist =  _context.Orders.Any(o => o.OrderNumber == orderNumber);

                if (!exist)
                {
                    return orderNumber;
                }

            }
        }
    }
}