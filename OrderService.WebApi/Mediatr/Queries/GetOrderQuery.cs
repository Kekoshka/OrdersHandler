using MediatR;
using OrderService.DataAccess.Postgres.Models.DTO;

namespace OrderService.WebApi.Mediatr.Queries
{
    /// <summary>
    /// Запрос для получения заказа
    /// </summary>
    public class GetOrderQuery : IRequest<OrderDTO>
    {
        /// <summary>
        /// Id заказа
        /// </summary>
        public long OrderId { get; set; }
    }
}
