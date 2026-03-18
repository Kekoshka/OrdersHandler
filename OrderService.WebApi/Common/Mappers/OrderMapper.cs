using OrderService.DataAccess.Postgres.Models;
using OrderService.DataAccess.Postgres.Models.DTO;
using OrderService.WebApi.Mediatr.Commands;
using OrderService.WebApi.Mediatr.Queries;
using Riok.Mapperly.Abstractions;

namespace OrderService.WebApi.Common.Mappers
{
    /// <summary>
    /// Маппер для объектов заказов
    /// </summary>
    [Mapper]
    public partial class OrderMapper
    {
        /// <summary>
        /// Преобразует DTO заказа в доменную модель заказа
        /// </summary>
        /// <param name="orderDTO">DTO заказа</param>
        /// <returns>Доменная модель заказа</returns>
        public partial Order DtoToOrder(OrderDTO orderDTO);
        
        /// <summary>
        /// Преобразует доменную модель заказа в DTO заказа
        /// </summary>
        /// <param name="order">доменная модель заказа</param>
        /// <returns>DTO заказа</returns>
        public partial OrderDTO OrderToDto(Order order);
        
        /// <summary>
        /// Преобразует команду создания заказа в DTO заказа
        /// </summary>
        /// <param name="createOrderCommand">команда созданя заказа</param>
        /// <returns>DTO заказа</returns>
        public partial OrderDTO CreateOrderCommandToDto(CreateOrderCommand createOrderCommand);

        /// <summary>
        /// Преобразует команду удаления заказа в DTO заказа
        /// </summary>
        /// <param name="deleteOrderCommand">Команда удаления заказа</param>
        /// <returns>DTO заказа</returns>
        public partial OrderDTO DeleteOrderCommandToDto(DeleteOrderCommand deleteOrderCommand);

        /// <summary>
        /// Преобразует команду получения заказа в DTO заказа
        /// </summary>
        /// <param name="getOrderQuery">команда получения заказа</param>
        /// <returns>DTO заказа</returns>
        public partial OrderDTO GetOrderQueryToDto(GetOrderQuery getOrderQuery);
    }
}
