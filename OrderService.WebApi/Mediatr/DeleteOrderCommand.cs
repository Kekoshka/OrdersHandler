using MediatR;
using OrderService.WebApi.Common.Mappers;
using OrderService.WebApi.Interfaces;

namespace OrderService.WebApi.Mediatr
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

    /// <summary>
    /// Обработчик для команды удаления заказа
    /// </summary>
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        OrderMapper _orderMapper;
        IOrderService _orderService;
        /// <summary>
        /// Инициализатор нового объекта обработчка команды удаления заказа
        /// </summary>
        /// <param name="orderMapper">Маппер объектов удаления заказа</param>
        /// <param name="orderService">Сервис заказов</param>
        public DeleteOrderCommandHandler(OrderMapper orderMapper, IOrderService orderService)
        {
            orderMapper = _orderMapper;
            _orderService = orderService;
        }

        /// <summary>
        /// Метод обработки команды удаления заказа
        /// </summary>
        /// <param name="request">Команда удаления заказа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            await _orderService.DeleteOrderAsync(request.OrderId, cancellationToken);
        }
    }
}
