using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.src.Data;
using OrderService.src.Interfaces;

namespace OrderService.src.Repository
{
    public class OrderRepository : IOrderInterface
    {
        private readonly DBContext _context;

        public OrderRepository(DBContext context)
        {
            _context = context;
        }


        //TODO: POST Crear Estacion (ADMIN)

        //TODO: Funcion para crar numero de pedido aleatorio sin repetirse

        //TODO: GET obtener order por id o por numero de pedido

        //TODO: PUT actualizar estado de un pedido (ADMIN)

        //TODO: PUT cancelar pedido cambiando el estado del mismo

        //TODO: GET Obtener Historia historico de pedidos de un cliente, Filtros por ID o numero de pedido, por rango de fecha de cracion
        
        //TODO: GET obtener historicos de clientes con filtros id o numero de pedido, rango de fechas de cracion, id o nombre de cliente (ADMIN)
    }
}