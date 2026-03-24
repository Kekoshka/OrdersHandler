namespace PaymentService.WebApi.Common.DTO
{
    /// <summary>
    /// DTO обновления статуса платежа
    /// </summary>
    public class UpdatePaymentDTO
    {
        /// <summary>
        /// Код платежа
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Статус платежа
        /// </summary>
        public bool Status { get; set; }
    }
}
