using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.src.Data;
using OrderService.src.Dto;
using OrderService.src.Helper;
using OrderService.src.Interfaces;
using OrderService.src.Mappers;
using OrderService.src.Messages;
using OrderService.src.Models;
using static OrderService.src.Messages.SenderMessage;

namespace OrderService.src.Repository
{
    /// <summary>
    /// Implementacion de patron Repository utilizado para encapsular las operaciones con la base de datos
    /// Maneja el acceso a datos y logica de negocio para orders
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly DBContext _context;

        private readonly IPublishEndpoint _publishEndpoint;

        /// <summary>
        /// Constructor del repositorio de órdenes.
        /// </summary>
        /// <param name="context">
        /// Contexto de base de datos utilizado para realizar operaciones relacionadas con irdenes.
        /// </param>
        /// <param name="publishEndpoint">
        /// Punto de publicación de mensajes para la comunicacion entre servicios.
        /// </param>
        public OrderRepository(DBContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }


        /// <summary>
        /// Metodo para crear una nueva orden.
        /// Tambiyn publica un mensaje en RabbitMQ para notificar la creacion al servicio de inventario.
        /// </summary>
        /// <param name="request">
        /// Dto con los datos de la orden a crear.
        /// </param>
        /// <returns>
        /// Dto con la informacion de la orden creada.
        /// </returns>
        public async Task<ResponseCreateOrderDto> CreateOrder(CreateOrderDto request)
        {
            var OrderRequest = new Order
            {
                UserId = request.UserId,
                UserName = request.UserName,
                UserEmail = request.UserEmail,
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

            

            await _context.Orders.AddAsync(OrderRequest);
            await _context.OrderItems.AddRangeAsync(OrderRequest.Items);
            await _context.SaveChangesAsync();

            //RabbitMQ
            var orderMessage = new SenderMessage
            {
                OrderId = OrderRequest.Id,
                Items = OrderRequest.Items.Select(i => new OrderItemsMessage
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()

            };

            await _publishEndpoint.Publish(orderMessage);

            return OrderRequest.ToCreateOrderResponse();
        }


        /// <summary>
        /// Metodo para obtener estado de una orden.
        /// </summary>
        /// <param name="OrderId">
        /// Identificador unico de orden.
        /// </param>
        /// <param name="orderNumber"><
        /// Numero de order.
        /// /param>
        /// <returns>
        /// Dto con el estado de la orden.
        /// </returns>
        public async Task<List<ResponseOrderStateDto>> GetOrderStateByIdentifier(Guid? Id, string? OrderNumber)
        {

            IQueryable<Order> query = _context.Orders;

            if (Id != null)
            {
                query = query.Where(o => o.UserId == Id);
            }

            if (!string.IsNullOrWhiteSpace(OrderNumber))
            {
                query = query.Where(o => o.OrderNumber == OrderNumber);
            }


            var orders = await query.ToListAsync();
            
            if(orders.Count == 0)
            {
                throw new Exception("Error: Sin Resultados");
            }



            return orders.Select(o =>  o.ToOrderStateResponse()).ToList();
        }


        /// <summary>
        /// Meotodo para cambiar el estado de una orden.
        /// </summary>
        /// <param name="OrderId">
        /// Identificador unico de orden.
        /// </param>
        /// <param name="OrderNumber">
        /// Numero de orden.
        /// </param>
        /// <param name="request">
        /// Dto con los nuevos datos.
        /// </param>
        /// <returns>
        /// Dto con con los datos de orden actualizados.
        /// </returns>
        public async Task<ResponseChangeStateDto> ChangeStateOrder(Guid? OrderId, string? OrderNumber, ChangeStateDto request)
        {

            var orderRequest = await GetOrderByIdOrOrderNumber(OrderId, OrderNumber);

            if (orderRequest == null)
            {
                throw new Exception("Error: Pedido no encontrado");
            }

            if (!string.IsNullOrWhiteSpace(request.TrackingNumber))
            {
                orderRequest.TrackingNumber = request.TrackingNumber;
            }

            orderRequest.OrderStatus = request.OrderStatus;
            orderRequest.UpdateAt = DateOnly.FromDateTime(DateTime.UtcNow);

            await _context.SaveChangesAsync();

            return orderRequest.ToChangeStateResponse();;


        }

        /// <summary>
        /// Metodo para cancelar una orden.
        /// </summary>
        /// <param name="OrderId">
        /// Identificador unico de orden (Puede ser nulo).
        /// </param>
        /// <param name="OrderNumber">
        /// Numero de orden.
        /// </param>
        /// <returns>
        /// Dto con con los datos de orden actualizados(Puede ser nulo).
        /// </returns>
        public async Task<ResponseChangeStateDto> CancelateOrder(Guid? OrderId, string? OrderNumber)
        {
            var orderRequest = await GetOrderByIdOrOrderNumber(OrderId, OrderNumber);

            if (orderRequest == null)
            {
                throw new Exception("Error: Pedido no encontrado");
            }
            if (orderRequest.OrderStatus == "Cancelado")
            {
                throw new Exception("Error: Pedido ya cancelado");
            }

            if (orderRequest.OrderStatus == "Enviado")
            {
                throw new Exception("Error: No es posible cancelar el pedido porque ya fue enviado.");
            }
            
            if (orderRequest.OrderStatus == "Entregado" )
            {
                throw new Exception("Error: No es posible cancelar el pedido porque ya fue entregado.");
            }

            orderRequest.OrderStatus = "Cancelado";
            orderRequest.UpdateAt = DateOnly.FromDateTime(DateTime.UtcNow);

            await _context.SaveChangesAsync();

            return orderRequest.ToChangeStateResponse(); ;

        }


        /// <summary>
        /// Metodo para obetener el listado de todas las ordenes de un usuario,
        /// con la posibilidad de aplicar filtros de busqueda (Usuarios).
        /// </summary>
        /// <param name="UserId">
        /// Identificador unico de cliente (usuario).
        /// </param>
        /// <param name="OrderId">
        /// Filtro opcional identificador unico de orden (Puede ser nulo).
        /// </param>
        /// <param name="OrderNumber">
        /// Filtro opcional numero de orden (Puede ser nulo).
        /// </param>
        /// <param name="InitalDate">
        ///  Filtro opcional de rango de fechas, fecha inicial (Puede ser nulo).
        /// </param>
        /// <param name="FinisDate">
        /// Filtro opcional de rango de fechas, fecha de termino (Puede ser nulo).
        /// </param>
        /// <returns>
        /// Dto con el listado de todos las ordenes del cliente con los filtros aplicados.
        /// </returns>
        public async Task<List<ResponseGetOrderUserDto>> GetAllOrdersUser(Guid UserId,Guid? OrderId, string? OrderNumber, DateOnly? InitialDate, DateOnly?FinishDate)
        {
            if (InitialDate.HasValue && FinishDate.HasValue && (InitialDate > FinishDate))
            {
                throw new Exception("Error: La fecha final debe ser mayor a la fecha inicial");
            }
                
            var query = _context.Orders.Where(o => o.UserId == UserId);

            query = OrderHelpers.UserFilter(query, OrderId, OrderNumber, InitialDate, FinishDate);

            var Orders = await query.Include(o => o.Items).Select(o => o.ToGetOrderUserResponse()).ToListAsync();

            if (Orders.Count == 0)
            {
                throw new Exception("Error: Usuario sin pedidos");
            }

            return Orders;
        }

        /// <summary>
        /// Metodo para obetener el listado de todas las ordenes, con la posibilidad  
        /// de aplicar filtros de busqueda (Admins).
        /// </summary>
        /// <param name="UserId">
        /// Filtro opcional identificador unico de cliente(Puede ser nulo).
        /// </param>
        /// <param name="Username">
        /// Filtro opcional Nombre de cliente (Puede ser nulo).
        /// </param>
        /// <param name="OrderId">
        /// Filtro opcional identificador unico de orden (Puede ser nulo).
        /// </param>
        /// <param name="OrderNumber">
        /// Filtro opcional numero de orden (Puede ser nulo).
        /// </param>
        /// <param name="InitialDate">
        /// Filtro opcional de rango de fechas, fecha de inicio (Puede ser nulo).
        /// </param>
        /// <param name="FinishDate">
        /// Filtro opcional de rango de fechas, fecha de termino (Puede ser nulo).
        /// </param>
        /// <returns>
        /// Dto con el listado de todos las ordenes con los filtros aplicados.
        /// </returns>

        public async Task<List<ResponseGetOrderAdminDto>> GetAllOrdersAdmin(Guid? UserId, string? Username, Guid? OrderId, string? OrderNumber, DateOnly? InitialDate, DateOnly? FinishDate)
        {
            if (InitialDate.HasValue && FinishDate.HasValue && (InitialDate > FinishDate))
            {
                throw new Exception("Error: La fecha final debe ser mayor a la fecha inicial");
            }

            IQueryable<Order> query = _context.Orders;

            query = OrderHelpers.AdminFilter(query, UserId, Username, OrderId, OrderNumber, InitialDate, FinishDate);

            var Orders = await query.Include(o => o.Items).Select(o => o.ToGetOrdersResponse()).ToListAsync();

            if (Orders.Count == 0)
            {
                throw new Exception("Error: Sin Resultados");
            }

            return Orders;
        }

        public async Task<string> GetUserEmail(Guid? OrderId, string? OrderNumber)
        {
            var orderRequest = await GetOrderByIdOrOrderNumber(OrderId, OrderNumber);

            if (orderRequest == null)
            {
                throw new Exception("Error: Email no encontrado");
            }

            return orderRequest.UserEmail;
        }


        /// <summary>
        /// Metodo para contar la cantidad de productos de una orden.
        /// </summary>
        /// <param name="Id">
        /// Identificador unico de orden.
        /// </param>
        /// <returns>
        /// Cantidad de productos en la ordern.
        /// </returns>
        public async Task<int> CountItemsOrderById(Guid Id)
        {
            var exist = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == Id);

            if (exist == null)
            {
                throw new Exception("Error: Orden no encontrada");
            }

            int count = exist.Items.Count;

            return count;

        }


        /// <summary>
        /// Obtiene una orden desde la base de datos utilizando su identificador unico o numero de orden.
        /// Permite incluir los items asociados a la orden si se especifica.
        /// </summary>
        /// <param name="OrderId">
        /// Identificador unico de la orden.
        /// </param>
        /// <param name="OrderNumber">
        /// Número de la orden.
        /// </param>
        /// <param name="includeItems">
        /// Indica si se deben incluir los items asociados a la orden en la consulta.
        /// </param>
        /// <returns>
        /// La entidad <see cref="Order"/> correspondiente si se encuentra en la base de datos; 
        /// de lo contrario, <c>null</c>.
        /// </returns>
        private async Task<Order?> GetOrderByIdOrOrderNumber(Guid? OrderId, string? OrderNumber, bool includeItems = false)
        {
            IQueryable<Order> query = _context.Orders;

            if (includeItems)
            {
                query = query.Include(o => o.Items);
            }

            if (OrderId != null)
            {
                return await query.FirstOrDefaultAsync(o => o.Id == OrderId);
            }

            if (!string.IsNullOrWhiteSpace(OrderNumber))
            {
                return await query.FirstOrDefaultAsync(o => o.OrderNumber == OrderNumber);
            }

            return null;
        }

        
        /// <summary>
        /// Genera un numero de orden unico de forma aleatoria con el formato <c>CEN-XXXX</c>.
        /// Verifica en la base de datos que el numero generado no exista previamente antes de retornarlo.
        /// </summary>
        /// <returns>
        /// Cadena de texto que representa el numero de pedido generado.
        /// </returns>
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