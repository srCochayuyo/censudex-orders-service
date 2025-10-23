using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.src.Dto;

namespace OrderService.src.Interfaces
{
    public interface IOrderRepository
    {
        public Task<ResponseOrderDto> CreateOrder(CreateOrderDto request);

        public Task<ResponseOrderDto?> GetOrderByIdorOrderNumber(Guid? OrderId, string? orderNumber);

        public Task<ResponseChangeStateDto> ChangeStateOrder(Guid? OrderId, string? OrderNumber, ChangeStateDto request);
    }
}