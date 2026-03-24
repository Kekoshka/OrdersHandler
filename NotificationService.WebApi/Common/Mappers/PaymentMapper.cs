using Microsoft.Extensions.Options;
using NotificationService.WebApi.Common.AvroSchemas;
using NotificationService.WebApi.Common.DTO;
using NotificationService.WebApi.Common.Options;
using Riok.Mapperly.Abstractions;
using System.Numerics;

namespace NotificationService.WebApi.Common.Mappers
{
    /// <summary>
    /// Маппинг данных о платежах
    /// </summary>
    [Mapper]
    public partial class PaymentMapper
    {
        /// <summary>
        /// Настройки типов данных, в частности decimal
        /// </summary>
        DataTypesOptions _dataTypesOptions;
        public PaymentMapper(IOptions<DataTypesOptions> dataTypesOptions)
        {
            _dataTypesOptions = dataTypesOptions.Value;
        }

        public partial Payment PaymentUpdatedToPayment(PaymentUpdated paymentUpdated);
        private long DateTimeToAvroLong(DateTime dateTime)
        {
            var utcDateTime = dateTime.ToUniversalTime();
            return new DateTimeOffset(utcDateTime).ToUnixTimeMilliseconds();
        }
        private decimal AvroBytesToDecimal(byte[] avroBytes)
        {
            byte[] littleEndian = avroBytes.Reverse().ToArray();
            BigInteger unscaled = new BigInteger(littleEndian);
            return (decimal)unscaled / (decimal)Math.Pow(10, _dataTypesOptions.DecimalScale);
        }

    }
}
