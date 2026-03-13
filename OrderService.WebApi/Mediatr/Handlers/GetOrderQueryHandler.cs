using MediatR;
using OrderService.DataAccess.Postgres.Models.DTO;
using OrderService.WebApi.Interfaces;
using OrderService.WebApi.Mappers;
using OrderService.WebApi.Mediatr.Queries;

namespace OrderService.WebApi.Mediatr.Handlers
{
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
            await _orderService.GetOrderAsync(request.OrderId,cancellationToken);
    }
}
