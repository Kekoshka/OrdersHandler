namespace PaymentService.WebApi.Common.DTO
{
    /// <summary>
    /// DTO создания платежа
    /// </summary>
    public class CreatePaymentDTO
    {
        /// <summary>
        /// Id заказа
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// Сумма платежа
        /// </summary>
        public decimal Price { get; set; }

    }
}
