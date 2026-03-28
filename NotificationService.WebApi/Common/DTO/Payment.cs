namespace NotificationService.WebApi.Common.DTO
{
    /// <summary>
    /// DTO заказа
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// Код платежа
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Код заказа
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// Сумма заказа
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Статус заказа
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Дата создания заказа
        /// </summary>
        public DateTime DateCreate { get; set; }
    }
}
