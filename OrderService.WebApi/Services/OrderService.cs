using ExceptionHandler.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderService.DataAccess.Postgres.Context;
using OrderService.WebApi.Common.DTO;
using OrderService.WebApi.Common.ExternalApi;
using OrderService.WebApi.Common.Mappers;
using OrderService.WebApi.Common.Options;
using OrderService.WebApi.Interfaces;

namespace OrderService.WebApi.Services
{
    /// <summary>
    /// Сервис обработки заказов
    /// </summary>
    public class OrderService : IOrderService
    {
        AppDbContext _context;
        OrderMapper _orderMapper;
        IPaymentServiceApi _paymentServiceApi;
        IKafkaService _kafkaService;
        ExternalServicesOptions _externalServicesOptions;

        /// <summary>
        /// Инициализация объекта сервиса обработки заказов
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="orderMapper">Маппер заказов</param>
        public OrderService(AppDbContext context,
            OrderMapper orderMapper,
            IPaymentServiceApi paymentServiceApi,
            IKafkaService kafkaService,
            IOptions<ExternalServicesOptions> externalServicesOptions) 
        {
            _context = context;
            _orderMapper = orderMapper;
            _paymentServiceApi = paymentServiceApi;
            _kafkaService = kafkaService;
            _externalServicesOptions = externalServicesOptions.Value;
        }
        
        /// <summary>
        /// Создание нового объекта заказа
        /// </summary>
        /// <param name="orderDto">DTO заказа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Id созданного заказа</returns>
        public async Task<long> CreateOrderAsync(OrderDTO orderDto, CancellationToken cancellationToken)
        {
            var order = _orderMapper.DtoToOrder(orderDto);
            await _context.Orders.AddAsync(order,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var message = _orderMapper.OrderToOrderCreated(order);
            await _kafkaService.ProduceAsync(_externalServicesOptions.OrderServiceTopic,
                message,
                order.Id,
                cancellationToken);


            await _paymentServiceApi.CreatePaymentAsync(new() //Сделать через маппер
                {
                    OrderId = order.Id,
                    Price = order.Price
                }, cancellationToken);

            return order.Id;
        }

        /// <summary>
        /// Получение объекта заказа по Id
        /// </summary>
        /// <param name="id">Id заказа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Объект заказа</returns>
        /// <exception cref="NotFoundException">Выбрасывается если заказ по указанному Id не найден</exception>
        public async Task<OrderDTO> GetOrderAsync(long id, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(id, cancellationToken) ?? throw new NotFoundException($"Order with id {id} not found");
            return _orderMapper.OrderToDto(order);
        }

        /// <summary>
        /// Удаление объекта заказа по указанному Id
        /// </summary>
        /// <param name="id">Id заказа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <exception cref="NotFoundException">Выбрасывается если объект заказа с указанным Id не найден</exception>
        public async Task DeleteOrderAsync(long id, CancellationToken cancellationToken) 
        {
            var deletedRows = await _context.Orders.Where(o => o.Id == id).ExecuteDeleteAsync(cancellationToken);
            if (deletedRows == 0)
                throw new NotFoundException($"Order with id {id} not found");

            _context.ChangeTracker.Clear();
        }
    }
}
