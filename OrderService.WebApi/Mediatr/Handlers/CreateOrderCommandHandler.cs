using MediatR;
using OrderService.WebApi.Interfaces;
using OrderService.WebApi.Mappers;
using OrderService.WebApi.Mediatr.Commands;

namespace OrderService.WebApi.Mediatr.Handlers
{
    /// <summary>
    /// Обработчик для команды создания заказа
    /// </summary>
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, long>
    {
        IOrderService _orderService;
        OrderMapper _orderMapper;
        /// <summary>
        /// Инициализация нового объекта обработчика команды создания заказа
        /// </summary>
        /// <param name="orderService">Сервис заказов</param>
        /// <param name="orderMapper">Маппер объектов заказов</param>
        public CreateOrderCommandHandler(IOrderService orderService, OrderMapper orderMapper)
        {
            _orderService = orderService;
            _orderMapper = orderMapper;
        }
        /// <summary>
        /// Метод обработки команды создания заказа
        /// </summary>
        /// <param name="request">Команда создания заказа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Id созданного заказа</returns>
        public async Task<long> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderDto = _orderMapper.CreateOrderCommandToDto(request);
            return await _orderService.CreateOrderAsync(orderDto, cancellationToken);
        }
    }
}
