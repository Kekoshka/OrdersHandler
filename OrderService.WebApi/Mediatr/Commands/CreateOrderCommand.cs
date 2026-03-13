using MediatR;

namespace OrderService.WebApi.Mediatr.Commands
{
    /// <summary>
    /// Команда создания заказа
    /// </summary>
    public class CreateOrderCommand : IRequest<long>
    {
        /// <summary>
        /// Id продукта
        /// </summary>
        public long ProductId { get; set; }
        
        /// <summary>
        /// Количество продуктов
        /// </summary>
        public int Amount { get; set; }
        
        /// <summary>
        /// Почтовый адрес клиента
        /// </summary>
        public string EmailClient { get; set; }
        
        /// <summary>
        /// Итоговая стоимость
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// Номер телефона заказчика
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}
