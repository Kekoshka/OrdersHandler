namespace NotificationService.WebApi.Common.DTO
{
    /// <summary>
    /// DTO заказа
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Код заказа
        /// </summary>
        public long Id;
        
        /// <summary>
        /// Код товара
        /// </summary>
        public long ProductId;
        
        /// <summary>
        /// Количество товаров
        /// </summary>
        public int Amount;
        
        /// <summary>
        /// Email заказчика
        /// </summary>
        public string EmailClient;
        
        /// <summary>
        /// Сумма заказа
        /// </summary>
        public decimal Price;
        
        /// <summary>
        /// Номер телефона заказчика
        /// </summary>
        public string PhoneNumber;
    }
}
