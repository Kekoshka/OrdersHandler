namespace OrderService.WebApi.Common.DTO
{
    /// <summary>
    /// DTO запроса на создание заказа
    /// </summary>
    public class CreatePaymentRequestDTO
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
