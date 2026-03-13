using MediatR;

namespace OrderService.WebApi.Mediatr.Commands
{
    /// <summary>
    /// Команда удаления заказа
    /// </summary>
    public class DeleteOrderCommand : IRequest
    {
        /// <summary>
        /// Id заказа
        /// </summary>
        public long OrderId { get; set; }
    }
}
