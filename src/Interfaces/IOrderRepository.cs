using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.src.Dto;

namespace OrderService.src.Interfaces
{
    public interface IOrderRepository
    {
        public Task<ResponseCreateOrderDto> CreateOrder(CreateOrderDto request);

        public Task<ResponseGetOrderDto?> GetOrderByIdentifier(Guid? OrderId, string? orderNumber);

        public Task<ResponseChangeStateDto> ChangeStateOrder(Guid? OrderId, string? OrderNumber, ChangeStateDto request);

        public Task<ResponseChangeStateDto> CancelateOrder(Guid? OrderId, string? OrderNumber);

        public Task<List<ResponseGetOrderDto>> GetAllOrdersUser(Guid UserId);
    }
}