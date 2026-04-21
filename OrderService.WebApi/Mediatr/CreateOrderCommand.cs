using MediatR;
using OrderService.WebApi.Common.Mappers;
using OrderService.WebApi.Interfaces;

namespace OrderService.WebApi.Mediatr
{
    /// <summary>
    /// Команда создания заказа
    /// </summary>
    public class CreateOrderCommand : IRequest<long>
    {
        /// <summary>
        /// Id продукта
        /// </summary>
        public long ProductId { get; set; }
        
        /// <summary>
        /// Количество продуктов
        /// </summary>
        public int Amount { get; set; }
        
        /// <summary>
        /// Почтовый адрес клиента
        /// </summary>
        public string EmailClient { get; set; }
        
        /// <summary>
        /// Итоговая стоимость
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// Номер телефона заказчика
        /// </summary>
        public string PhoneNumber { get; set; }
    }

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
