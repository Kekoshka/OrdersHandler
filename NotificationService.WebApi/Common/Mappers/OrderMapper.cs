using Microsoft.Extensions.Options;
using NotificationService.WebApi.DTO;
using NotificationService.WebApi.Common.Options;
using Riok.Mapperly.Abstractions;
using System.Numerics;
using NotificationService.WebApi.Common.AvroSchemas;

namespace NotificationService.WebApi.Common.Mappers
{
    /// <summary>
    /// Маппинг данных о заказах
    /// </summary>
    [Mapper]
    public partial class OrderMapper
    {
        /// <summary>
        /// Настройки типов данных, в частности decimal
        /// </summary>
        DataTypesOptions _dataTypesOptions;
        public OrderMapper(IOptions<DataTypesOptions> dataTypesOptions)
        {
            _dataTypesOptions = dataTypesOptions.Value;
        }

        /// <summary>
        /// Маппит DTO события о создании заказа на DTO заказа
        /// </summary>
        /// <param name="orderCreated"></param>
        /// <returns></returns>
        public partial Order OrderCreatedToOrder(OrderCreated orderCreated);
        private decimal AvroBytesToDecimal(byte[] avroBytes)
        {
            byte[] littleEndian = avroBytes.Reverse().ToArray();
            BigInteger unscaled = new BigInteger(littleEndian);
            return (decimal)unscaled / (decimal)Math.Pow(10, _dataTypesOptions.DecimalScale);
        }

    }
}
