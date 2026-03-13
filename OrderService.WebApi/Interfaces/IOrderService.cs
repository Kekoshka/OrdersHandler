using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres.Context;
using OrderService.DataAccess.Postgres.Models;
using OrderService.DataAccess.Postgres.Models.DTO;
using OrderService.WebApi.Mappers;
using OrderService.WebApi.Mappers.CustomExceptions;

namespace OrderService.WebApi.Interfaces
{
    /// <summary>
    /// Определяет контракт для сервиса работы с заказами
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Создает новый заказ на основе переданных данных
        /// </summary>
        /// <param name="orderDto">Объект с данными для создания заказа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Id заказа</returns>
        public Task<long> CreateOrderAsync(OrderDTO orderDto, CancellationToken cancellationToken);
        
        /// <summary>
        /// Получает данные заказа по его Id
        /// </summary>
        /// <param name="id">Id заказа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Данные заказа</returns>
        public Task<OrderDTO> GetOrderAsync(long id, CancellationToken cancellationToken);
        
        /// <summary>
        /// Удаляет заказ по его Id 
        /// </summary>
        /// <param name="id">Id заказа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Ответ 204 NoContent при успешном удалении заказа</returns>
        public Task DeleteOrderAsync(long id, CancellationToken cancellationToken);
    }
}
