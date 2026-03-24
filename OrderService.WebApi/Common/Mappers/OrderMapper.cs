using Avro;
using Avro.Util;
using Microsoft.Extensions.Options;
using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.Common.DTO;
using OrderService.WebApi.Mediatr;
using PaymentService.WebApi.Common.Options;
using Riok.Mapperly.Abstractions;
using System.Numerics;

namespace OrderService.WebApi.Common.Mappers
{
    /// <summary>
    /// Маппер для объектов заказов
    /// </summary>
    [Mapper]
    public partial class OrderMapper
    {
        DataTypesOptions _dataTypesOptions;
        public OrderMapper(IOptions<DataTypesOptions> dataTypesOptions) 
        {
            _dataTypesOptions = dataTypesOptions.Value;
        }

        /// <summary>
        /// Преобразует DTO заказа в доменную модель заказа
        /// </summary>
        /// <param name="orderDTO">DTO заказа</param>
        /// <returns>Доменная модель заказа</returns>
        public partial Order DtoToOrder(OrderDTO orderDTO);
        
        /// <summary>
        /// Преобразует доменную модель заказа в DTO заказа
        /// </summary>
        /// <param name="order">доменная модель заказа</param>
        /// <returns>DTO заказа</returns>
        public partial OrderDTO OrderToDto(Order order);
        
        /// <summary>
        /// Преобразует команду создания заказа в DTO заказа
        /// </summary>
        /// <param name="createOrderCommand">команда созданя заказа</param>
        /// <returns>DTO заказа</returns>
        public partial OrderDTO CreateOrderCommandToDto(CreateOrderCommand createOrderCommand);

        /// <summary>
        /// Преобразует команду удаления заказа в DTO заказа
        /// </summary>
        /// <param name="deleteOrderCommand">Команда удаления заказа</param>
        /// <returns>DTO заказа</returns>
        public partial OrderDTO DeleteOrderCommandToDto(DeleteOrderCommand deleteOrderCommand);

        /// <summary>
        /// Преобразует команду получения заказа в DTO заказа
        /// </summary>
        /// <param name="getOrderQuery">команда получения заказа</param>
        /// <returns>DTO заказа</returns>
        public partial OrderDTO GetOrderQueryToDto(GetOrderQuery getOrderQuery);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns>сообщение о создании заказа</returns>
        public partial OrderCreated OrderToOrderCreated(Order order);

        private byte[] DecimalToAvroBytes(decimal value)
        {
            decimal scaledValue = value * (decimal)Math.Pow(10, _dataTypesOptions.DecimalScale);
            BigInteger unscaled = new BigInteger(scaledValue);
            byte[] littleEndian = unscaled.ToByteArray();
            byte[] bigEndian = littleEndian.Reverse().ToArray();
            return bigEndian;
        }
    }
}
