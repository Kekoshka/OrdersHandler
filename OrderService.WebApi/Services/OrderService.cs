using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres.Context;
using OrderService.DataAccess.Postgres.Models;
using OrderService.DataAccess.Postgres.Models.DTO;
using OrderService.WebApi.Interfaces;
using OrderService.WebApi.Mappers;
using OrderService.WebApi.Mappers.CustomExceptions;

namespace OrderService.WebApi.Services
{
    /// <summary>
    /// Сервис обработки заказов
    /// </summary>
    public class OrderService : IOrderService
    {
        AppDbContext _context;
        OrderMapper _orderMapper;
        
        /// <summary>
        /// Инициализация объекта сервиса обработки заказов
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="orderMapper">Маппер заказов</param>
        public OrderService(AppDbContext context, OrderMapper orderMapper) 
        {
            _context = context;
            _orderMapper = orderMapper;
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
            await _context.Orders.AddAsync(order);
            _context.SaveChanges();
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
            var order = await _context.Orders.FindAsync(id) ?? throw new NotFoundException($"Order with id {id} not found");
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
            var deletedRows = await _context.Orders.Where(o => o.Id == id).ExecuteDeleteAsync();
            if (deletedRows == 0)
                throw new NotFoundException($"Order with id {id} not found");
        }
    }
}
