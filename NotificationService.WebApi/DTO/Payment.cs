namespace NotificationService.WebApi.DTO
{
    /// <summary>
    /// DTO заказа
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// Код платежа
        /// </summary>
        public long Id;
        
        /// <summary>
        /// Код заказа
        /// </summary>
        public long OrderId;
        
        /// <summary>
        /// Сумма заказа
        /// </summary>
        public decimal Price;
        
        /// <summary>
        /// Статус заказа
        /// </summary>
        public bool Status;
        
        /// <summary>
        /// Дата создания заказа
        /// </summary>
        public DateTime DateCreate;
    }
}
