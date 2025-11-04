using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.src.Models;

namespace OrderService.src.Helper
{
    /// <summary>
    /// Clase helper el cual proporciona metodos externos para las operaciones de ordenes.
    /// Creado con el fin a evitar codigo duplicado.
    /// </summary>
    public class OrderHelpers
    {

        /// <summary>
        /// Metodo estatico que distingue entre dos tipos de identificadores de una orden.
        /// </summary>
        /// <param name="orderIdentifier">
        /// Identificador de orden el cual puede ser el ID de orden o de cliente (Usuario)
        /// </param>
        /// <returns>
        /// Tupla que contiene el valor identificado.
        /// </returns>
        public static (Guid? Id, string? OrderNumber) ParseOrderIdentifier(string? orderIdentifier)
        {
            Guid? Id = null;
            string? OrderNumber = null;

            if (Guid.TryParse(orderIdentifier, out var guid))
            {
                Id = guid;

            }
            else
            {
                OrderNumber = orderIdentifier;
            }

            return (Id, OrderNumber);

        }

        /// <summary>
        /// Metodo estatico para aplicar filtros opcionales a una consulta IQueryable de ordenes (Admin).
        /// Los filtros se aplican de manera condicional solo si los parametros tienen valores.
        /// Metodo
        /// </summary>
        /// <param name="query">
        /// Consulta base de orders a filtrar.
        /// </param>
        /// <param name="UserId">
        /// Filtro opcional por Identificador unico de usuario.
        /// </param>
        /// <param name="UserName">
        /// Filtro opcional por nombre de usuario.
        /// </param>
        /// <param name="OrderId">
        /// Filtro opcional por Identificador unico de orden.
        /// </param>
        /// <param name="OrderNumber">
        /// Filtro opcional por numero de orden.
        /// </param>
        /// <param name="InitialDate">
        /// Filtro opcional por intervalos de fechas, fecha inicial.
        /// </param>
        /// <param name="FinishDate">
        /// Filtro opcional por intervalos de fechas, fecha de termino.
        /// </param>
        /// <returns>
        /// Query IQueryable con los filtros aplicados
        /// </returns>
        public static IQueryable<Order> AdminFilter(IQueryable<Order> query, Guid? UserId, string? UserName, Guid? OrderId, string? OrderNumber, DateOnly? InitialDate, DateOnly? FinishDate)
        {
            if (UserId != null)
            {
                query = query.Where(o => o.UserId == UserId);
            }

            if (!string.IsNullOrWhiteSpace(UserName))
            {
                query = query.Where(o => o.UserName.ToLower().Trim() == UserName.ToLower().Trim());
            }

            if (OrderId != null)
            {
                query = query.Where(o => o.Id == OrderId);
            }

            if (!string.IsNullOrWhiteSpace(OrderNumber))
            {
                query = query.Where(o => o.OrderNumber == OrderNumber);
            }

            if (InitialDate != null)
            {
                query = query.Where(o => o.CreateAt >= InitialDate);
            }

            if (FinishDate != null)
            {
                query = query.Where(o => o.CreateAt <= FinishDate);
            }

            return query;

        }  

        /// <summary>
        /// Metodo estatico para aplicar filtros opcionales a una consulta IQueryable de ordenes (User).
        /// Los filtros se aplican de manera condicional solo si los parametros tienen valores.
        /// Metodo
        /// </summary>
        /// <param name="query">
        /// Consulta base de orders a filtrar.
        /// </param>
        /// <param name="OrderId">
        /// Filtro opcional por Identificador unico de orden.
        /// </param>
        /// <param name="OrderNumber">
        /// Filtro opcional por numero de orden.
        /// </param>
        /// <param name="InitialDate">
        /// Filtro opcional por intervalos de fechas, fecha inicial.
        /// </param>
        /// <param name="FinishDate">
        /// Filtro opcional por intervalos de fechas, fecha de termino.
        /// </param>
        /// <returns>
        /// Query IQueryable con los filtros aplicados
        /// </returns>
        public static IQueryable<Order> UserFilter(IQueryable<Order> query, Guid? OrderId, string? OrderNumber, DateOnly? InitialDate, DateOnly? FinishDate)
        {
            if (OrderId != null)
            {
                query = query.Where(o => o.Id == OrderId);
            }

            if (!string.IsNullOrWhiteSpace(OrderNumber))
            {
                query = query.Where(o => o.OrderNumber == OrderNumber);
            }

            if (InitialDate != null)
            {
                query = query.Where(o => o.CreateAt >= InitialDate);
            }

            if (FinishDate != null)
            {
                query = query.Where(o => o.CreateAt <= FinishDate);
            }

            return query;

        }  
    }
}