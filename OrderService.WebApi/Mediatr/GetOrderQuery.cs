using MediatR;
using OrderService.WebApi.Common.DTO;
using OrderService.WebApi.Common.Mappers;
using OrderService.WebApi.Interfaces;

namespace OrderService.WebApi.Mediatr
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

    /// <summary>
    /// Обработчик для запроса получения заказа
    /// </summary>
    public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderDTO>
    {
        OrderMapper _orderMapper;
        IOrderService _orderService;
        /// <summary>
        /// Инициализатор нового объекта обработчика запроса получения заказа
        /// </summary>
        /// <param name="orderMapper">Маппер объектов заказов</param>
        /// <param name="orderService">Сервис заказов</param>
        public GetOrderQueryHandler(OrderMapper orderMapper, IOrderService orderService)
        {
            _orderMapper = orderMapper;
            _orderService = orderService;
        }
        /// <summary>
        /// Обработчик для запроса получения заказа
        /// </summary>
        /// <param name="request">Запрос получения заказа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>DTO заказа</returns>
        public async Task<OrderDTO> Handle(GetOrderQuery request, CancellationToken cancellationToken) =>
            await _orderService.GetOrderAsync(request.OrderId, cancellationToken);
    }
}
