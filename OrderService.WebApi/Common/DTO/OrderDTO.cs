namespace OrderService.WebApi.Common.DTO
{
    /// <summary>
    /// DTO заказа
    /// </summary>
    public class OrderDTO
    {
        /// <summary>
        /// Код товара
        /// </summary>
        public long ProductId { get; set; }
        
        /// <summary>
        /// Количество товаров
        /// </summary>
        public int Amount { get; set; }
        
        /// <summary>
        /// Email заказчика
        /// </summary>
        public string EmailClient { get; set; }
        
        /// <summary>
        /// Сумма заказа
        /// </summary>
        public double Price { get; set; }
        
        /// <summary>
        /// Номер телефона заказчика
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}
