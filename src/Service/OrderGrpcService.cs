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
    public class OrderGrpcService : Grpc.OrderService.OrderServiceBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderGrpcService> _logger;

        public OrderGrpcService(IOrderRepository orderRepository, ILogger<OrderGrpcService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }


        //Crear orden
        public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "UserId es requerido"));
                }
                    

                if (string.IsNullOrWhiteSpace(request.UserName))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "UserName es requerido"));
                }
                    

                if (string.IsNullOrWhiteSpace(request.Address))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Direccion es requerido"));
                }
                    

                if (request.Items == null || request.Items.Count == 0)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Items es requerido"));
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

                return ProtoMappers.ToCreateOrderProtoResponse(result);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creando orden");
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }

        //Obtener estado por identificador
        public override async Task<GetOrderStatusResponse> GetOrderStatus(GetOrderStatusRequest request, ServerCallContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Identifier))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Identifier de pedido es requerido (Id de usuario o Numero de pedido)"));
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

        // cambiar estado de orden
        public override async Task<ChangeOrderStateResponse> ChangeOrderState(ChangeOrderStateRequest request,ServerCallContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Identifier))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Identifier es requerido"));
                }

                if (string.IsNullOrWhiteSpace(request.OrderStatus))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Nuevo estado de orden es requerido"));
                }

                var validStates = new[] { "pendiente", "en procesamiento", "enprocesamiento", "enviado", "entregado" };
                if (!validStates.Contains(request.OrderStatus.ToLower().Replace(" ", "")))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Estado inválido. Valores permitidos: Pendiente, En Procesamiento, Enviado, Entregado"));
                }


                var (OrderId, OrderNumber) = OrderHelpers.ParseOrderIdentifier(request.Identifier);

                var requestDto = new ChangeStateDto
                {
                    OrderStatus = request.OrderStatus,
                    TrackingNumber = request.TrackingNumber
                };


                var result = await _orderRepository.ChangeStateOrder(OrderId, OrderNumber, requestDto);

                return ProtoMappers.ToChangeOrderStateProtoResponse(result);


            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al cambiar estado de pedido");
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }

        // Cancelar orden
        public override async Task<CancelOrderResponse> CancelOrder(CancelOrderRequest request, ServerCallContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Identifier))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Identifier es requerido"));
                }

                var (OrderId, OrderNumber) = OrderHelpers.ParseOrderIdentifier(request.Identifier);

                var result = await _orderRepository.CancelateOrder(OrderId, OrderNumber);

                return ProtoMappers.ToCancelOrderProtoResponse(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al cancelar pedido");
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }

        //Obtener historico de pedidos de un usuario con filtos (usuarios)
        public override async Task<GetUserOrdersResponse> GetUserOrders(GetUserOrdersRequest request, ServerCallContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "UserId es requerido"));
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
            catch(Exception e)
            {
                _logger.LogError(e, "Error al obtener pedidos ");
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
            
        }
        
        //Obtener historico de pedidos de todos los usuarios con filtros (Admins)
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
                
                _logger.LogInformation("GRPC - UserId: {UserId}, UserName: '{UserName}', OrderId: {OrderId}, OrderNumber: '{OrderNumber}', InitialDate: {InitialDate}, FinishDate: {FinishDate}", 
                    userId, userName, OrderId, OrderNumber, initialDate, finishDate);
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