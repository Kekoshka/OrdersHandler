using NotificationService.WebApi.Common.DTO;
using Refit;

namespace NotificationService.WebApi.Common.ExternalApi
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
        [Get("/api/orders/{orderId}")]
        Task<GetOrderRequestDTO> GetOrderAsync(long orderId, CancellationToken cancellationToken);
    }
}
