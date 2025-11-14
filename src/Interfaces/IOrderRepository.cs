using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.src.Dto;
using OrderService.src.Models;

namespace OrderService.src.Interfaces
{
    /// <summary>
    /// Interfaz de repositorio de ordenes.
    /// Define la forma en que se obtiene y gestiona la informacion de ordenes.
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Metodo para crear una nueva orden.
        /// </summary>
        /// <param name="request">
        /// Dto con los datos de la orden a crear.
        /// </param>
        /// <returns>
        /// Dto con la informacion de la orden creada.
        /// </returns>
        public Task<ResponseCreateOrderDto> CreateOrder(CreateOrderDto request);

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
        public Task<List<ResponseOrderStateDto>> GetOrderStateByIdentifier(Guid? OrderId, string? orderNumber);

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
        public Task<ResponseChangeStateDto> ChangeStateOrder(Guid? OrderId, string? OrderNumber, ChangeStateDto request);

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
        public Task<ResponseChangeStateDto> CancelateOrder(Guid? OrderId, string? OrderNumber);

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
        public Task<List<ResponseGetOrderUserDto>> GetAllOrdersUser(Guid UserId, Guid? OrderId, string? OrderNumber, DateOnly? InitalDate, DateOnly? FinisDate);

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
        public Task<List<ResponseGetOrderAdminDto>> GetAllOrdersAdmin(Guid? UserId, string? Username, Guid? OrderId, string? OrderNumber, DateOnly? InitialDate, DateOnly? FinishDate);

        /// <summary>
        /// Metodo para contar la cantidad de productos de una orden.
        /// </summary>
        /// <param name="Id">
        /// Identificador unico de orden.
        /// </param>
        /// <returns>
        /// Cantidad de productos en la ordern.
        /// </returns>
        public Task<int> CountItemsOrderById(Guid Id);

        /// <summary>
        /// Obtiene el email del usuario ligado a la orden utilizando el id de orden o numero de orden.
        /// </summary>
        /// <param name="OrderId">
        /// Identificador unico de la orden.
        /// </param>
        /// <param name="OrderNumber">
        /// Número de la orden.
        /// <returns>
        /// El Email del usuario
        /// </returns>
        public Task<string> GetUserEmail(Guid? OrderId, string? OrderNumber);

    }
}