using NotificationService.WebApi.DTO;
using Refit;

namespace NotificationService.WebApi.Interfaces.ExternalApi
{
    /// <summary>
    /// Методы для общения с сервисом заказов
    /// </summary>
    public interface IOrderServiceApi
    {
        /// <summary>
        /// Получение заказа из сервиса заказов по id
        /// </summary>
        /// <param name="orderId">Id заказа</param>
        /// <param name="cancellationToken">токен отмены операции</param>
        /// <returns></returns>
        [Post("/api/orders/get")]
        Task<GetOrderRequestDTO> GetOrderAsync(long orderId, CancellationToken cancellationToken);
    }
}
