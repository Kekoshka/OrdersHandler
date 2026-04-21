using PaymentService.WebApi.Common.DTO;

namespace PaymentService.WebApi.Interfaces
{
    /// <summary>
    /// Определяет контракты для сервиса работы сплатежами
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Создать новый платеж
        /// </summary>
        /// <param name="createPaymentDTO">DTO создания платежа</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Код платежа</returns>
        public Task<long> CreatePaymentAsync(CreatePaymentDTO createPaymentDTO, CancellationToken cancellationToken);
        
        /// <summary>
        /// Обновить статус платежа
        /// </summary>
        /// <remarks>
        /// 0 - не оплачен
        /// 1 - оплачен
        /// </remarks>
        /// <param name="paymentId">Код платежа</param>
        /// <param name="status">Новый статус платежа</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        public Task UpdatePaymentAsync(long paymentId, bool status, CancellationToken cancellationToken);
        
        /// <summary>
        /// Получить DTO платежа по коду
        /// </summary>
        /// <param name="paymentId">Код платежа</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>DTO платежа</returns>
        public Task<GetPaymentDTO> GetPaymentAsync(long paymentId, CancellationToken cancellationToken);
    }
}
