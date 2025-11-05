using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using MassTransit;
using OrderService.Grpc;
using OrderService.src.Dto;
using OrderService.src.Helper;
using OrderService.src.Interfaces;
using OrderService.src.Mappers;
using RabbitMQ.Client;


namespace OrderService.src.Service
{
    /// <summary>
    /// Servicio gRPC encargado de gestionar las operaciones relacionadas con ordenes.
    /// Implementa las operaciones definidas en el contrato gRPC `OrderService`.
    /// </summary>
    public class OrderGrpcService : Grpc.OrderService.OrderServiceBase
    {
        private readonly IOrderRepository _orderRepository;

        private SendGridService _sendGrid;
        private readonly ILogger<OrderGrpcService> _logger;

        /// <summary>
        /// Constructor del servicio de ordenes gRPC.
        /// Inicializa las dependencias necesarias para la interacción con el repositorio y el registro de logs.
        /// </summary>
        /// <param name="orderRepository">
        /// Repositorio encargado de la gestion de las ordenes.
        /// </param>
        /// <param name="logger">
        /// Instancia de logger para registrar información y errores.
        /// </param>
        public OrderGrpcService(IOrderRepository orderRepository,SendGridService sendGrid, ILogger<OrderGrpcService> logger)
        {
            _orderRepository = orderRepository;
            _sendGrid = sendGrid;
            _logger = logger;
        }

        /// <summary>
        /// Crea una nueva orden a partir de los datos recibidos desde el cliente gRPC.
        /// Valida los campos de entrada antes de persistir la orden.
        /// </summary>
        /// <param name="request">
        /// Datos necesarios para la creacion de la orden.
        /// </param>
        /// <param name="context">
        /// Contexto de la llamada gRPC.
        /// </param>
        /// <returns>
        /// Objeto con los datos de la orden creada.
        /// </returns>
        public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Error. Id de usuario es requerido"));
                }

                if (string.IsNullOrWhiteSpace(request.UserName))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Error. Nombre de usuario requerido"));
                }

                if (string.IsNullOrWhiteSpace(request.Address))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Error. Direccion requerida"));
                }

                if (request.Items == null || request.Items.Count == 0)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Error. Items requeridos"));
                }

                if (request.Items.Any(i => string.IsNullOrWhiteSpace(i.ProductId)))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Error. Producto sin Id"));
                }

                if (request.Items.Any(i => string.IsNullOrWhiteSpace(i.ProductName)))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Error. Producto sin nombre de usuario"));
                }

                if (request.Items.Any(i => i.Quantity <= 0))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Error. La cantidad de cada producto debe ser mayor a 0"));
                }

                if (request.Items.Any(i => i.UnitPrice <= 0))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Error. El precio de cada producto debe ser mayor a 0"));
                }

                var createOrderRequest = new CreateOrderDto
                {
                    UserId = Guid.Parse(request.UserId),
                    UserName = request.UserName,
                    Address = request.Address,
                    Items = request.Items.Select(i => new CreateOrderItemDto
                    {
                        ProductId = Guid.Parse(i.ProductId),
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList()
                };

                var result = await _orderRepository.CreateOrder(createOrderRequest);

                //Enviarnotificacion
                await _sendGrid.SendCreateOrderEmail(request.UserEmail, result.OrderNumber, result.UserName, result.TotalPrice);

                return ProtoMappers.ToCreateOrderProtoResponse(result);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creando orden");
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }

        /// <summary>
        /// Obtiene el estado actual de una orden a partir de su identificador.
        /// El identificador puede corresponder al ID de  o al numero de pedido.
        /// </summary>
        /// <param name="request">
        /// Solicitud que contiene el identificador de la orden.
        /// </param>
        /// <param name="context">
        /// Contexto de la llamada gRPC.
        /// </param>
        /// <returns>
        /// Objeto con el estado de la orden.
        /// </returns>
        public override async Task<GetOrderStatusResponse> GetOrderStatus(GetOrderStatusRequest request, ServerCallContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Identifier))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Error. Identifier de pedido es requerido (Id de usuario o Numero de pedido)"));
                }

                var (OrderId, OrderNumber) = OrderHelpers.ParseOrderIdentifier(request.Identifier);

                var OrderRequest = await _orderRepository.GetOrderStateByIdentifier(OrderId, OrderNumber);

                return ProtoMappers.ToGetOrderStatusProtoResponse(OrderRequest);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error obteniendo estado de pedido");
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }

        /// <summary>
        /// Cambia el estado de una orden existente en el sistema.
        /// </summary>
        /// <param name="request">
        /// Solicitud que contiene el identificador y el nuevo estado de la orden.
        /// </param>
        /// <param name="context">Contexto de la llamada gRPC.</param>
        /// <returns>
        /// Objeto con los datos de la orden actualizada.
        /// </returns>
        public override async Task<ChangeOrderStateResponse> ChangeOrderState(ChangeOrderStateRequest request, ServerCallContext context)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(request.Identifier))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, " Error Identifier requerido"));
                }

                if (string.IsNullOrWhiteSpace(request.OrderStatus))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Error. Nuevo estado de orden requerido"));
                }

                var validStates = new[] { "pendiente", "en procesamiento", "enprocesamiento", "enviado", "entregado" };
                if (!validStates.Contains(request.OrderStatus.ToLower().Replace(" ", "")))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Error. Estado inválido. Valores permitidos: Pendiente, En Procesamiento, Enviado, Entregado"));
                }

                if (string.IsNullOrWhiteSpace(request.TrackingNumber) && request.OrderStatus.ToLower() == "enviado")
                {
                    throw new Exception("Error: El numero de seguimiento es requerido para cambiar el estado a Enviado");
                }

                if (!string.IsNullOrWhiteSpace(request.TrackingNumber) && request.OrderStatus.ToLower() != "enviado")
                {
                    throw new Exception("Error: El número de seguimiento solo puede asignarse cuando el estado es Enviado.");
                }

                var (OrderId, OrderNumber) = OrderHelpers.ParseOrderIdentifier(request.Identifier);

                var requestDto = new ChangeStateDto
                {
                    OrderStatus = request.OrderStatus,
                    TrackingNumber = request.TrackingNumber
                };

                var result = await _orderRepository.ChangeStateOrder(OrderId, OrderNumber, requestDto);

                //Enviar notificacion
                await _sendGrid.SendchangeStateEmail(request.UserEmail, result.OrderNumber, request.OrderStatus,request.TrackingNumber);

                return ProtoMappers.ToChangeOrderStateProtoResponse(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al cambiar estado de pedido");
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }

        /// <summary>
        /// Cancela una orden existente utilizando su identificador.
        /// </summary>
        /// <param name="request">
        /// Solicitud que contiene el identificador de la orden a cancelar.
        /// </param>
        /// <param name="context">
        /// Contexto de la llamada gRPC.
        /// </param>
        /// <returns>
        /// Objeto con la información de la orden cancelada.
        /// </returns>
        public override async Task<CancelOrderResponse> CancelOrder(CancelOrderRequest request, ServerCallContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Identifier))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Error. Identifier requerido"));
                }

                var (OrderId, OrderNumber) = OrderHelpers.ParseOrderIdentifier(request.Identifier);

                var result = await _orderRepository.CancelateOrder(OrderId, OrderNumber);

                //Enviar notificacion
                await _sendGrid.SendCancelOrderEmail(request.UserEmail,result.OrderNumber,request.Reason);

                return ProtoMappers.ToCancelOrderProtoResponse(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al cancelar pedido");
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }

        /// <summary>
        /// Obtiene el historial de ordenes de un usuario con la posibilidad de aplicar filtros.
        /// </summary>
        /// <param name="request">
        /// Solicitud que contiene el Id del usuario y los filtros opcionales.
        /// </param>
        /// <param name="context">
        /// Contexto de la llamada gRPC.
        /// </param>
        /// <returns>
        /// Objeto con la lista de órdenes del usuario.
        /// </returns>
        public override async Task<GetUserOrdersResponse> GetUserOrders(GetUserOrdersRequest request, ServerCallContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Error. Id de usuario requerido"));
                }

                var userId = Guid.Parse(request.UserId);
                var (OrderId, OrderNumber) = OrderHelpers.ParseOrderIdentifier(request.OrderIdentifier);

                DateOnly? initialDate = null;
                if (!string.IsNullOrWhiteSpace(request.InitialDate))
                {
                    initialDate = DateOnly.Parse(request.InitialDate);
                }

                DateOnly? finishDate = null;
                if (!string.IsNullOrWhiteSpace(request.FinishDate))
                {
                    finishDate = DateOnly.Parse(request.FinishDate);
                }

                var orders = await _orderRepository.GetAllOrdersUser(userId, OrderId, OrderNumber, initialDate, finishDate);

                return ProtoMappers.ToGetUserOrdersProtoResponse(orders);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al obtener pedidos ");
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }

        }

        /// <summary>
        /// Obtiene el historial de ordenes de todos los usuarios con filtros aplicables (Admin).
        /// </summary>
        /// <param name="request">
        /// Solicitud con filtros opcionales como identificador de usuario, nombre, fechas o número de orden.
        /// </param>
        /// <param name="context">
        /// Contexto de la llamada gRPC.
        /// </param>
        /// <returns>
        /// Objeto con la lista de ordenes filtrados.
        /// </returns>
        public override async Task<GetAdminOrdersResponse> GetAdminOrders(GetAdminOrdersRequest request, ServerCallContext context)
        {
            try
            {
                Guid? userId = null;
                string? userName = null;

                if (Guid.TryParse(request.UserIdentifier, out var guid))
                {
                    userId = guid;
                }
                else
                {
                    userName = request.UserIdentifier;
                }

                var (OrderId, OrderNumber) = OrderHelpers.ParseOrderIdentifier(request.OrderIdentifier);

                DateOnly? initialDate = null;
                if (!string.IsNullOrWhiteSpace(request.InitialDate))
                {
                    initialDate = DateOnly.Parse(request.InitialDate);
                }

                DateOnly? finishDate = null;
                if (!string.IsNullOrWhiteSpace(request.FinishDate))
                {
                    finishDate = DateOnly.Parse(request.FinishDate);
                }

                var orders = await _orderRepository.GetAllOrdersAdmin(userId, userName, OrderId, OrderNumber, initialDate, finishDate);

                return ProtoMappers.ToGetAdminOrdersProtoResponse(orders);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al obtener pedidos ");
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }
    }

}