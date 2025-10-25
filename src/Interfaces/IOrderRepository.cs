using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.src.Dto;
using OrderService.src.Models;

namespace OrderService.src.Interfaces
{
    public interface IOrderRepository
    {
        public Task<ResponseCreateOrderDto> CreateOrder(CreateOrderDto request);

        public Task<List<ResponseOrderStateDto>> GetOrderStateByIdentifier(Guid? OrderId, string? orderNumber);

        public Task<ResponseChangeStateDto> ChangeStateOrder(Guid? OrderId, string? OrderNumber, ChangeStateDto request);

        public Task<ResponseChangeStateDto> CancelateOrder(Guid? OrderId, string? OrderNumber);

        public Task<List<ResponseGetOrderUserDto>> GetAllOrdersUser(Guid UserId, Guid? OrderId, string? OrderNumber, DateOnly? InitalDate, DateOnly? FinisDate);

        public Task<List<ResponseGetOrderAdminDto>> GetAllOrdersAdmin(Guid? UserId, string? Username, Guid? OrderId, string? OrderNumber, DateOnly? InitialDate, DateOnly? FinishDate);

        public Task<int> CountItemsOrderById(Guid Id);

    }
}