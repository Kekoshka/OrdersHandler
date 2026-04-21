using Microsoft.Extensions.Options;
using PaymentService.DataAccess.Postgres.Models;
using PaymentService.WebApi.Common.DTO;
using PaymentService.WebApi.Common.Options;
using PaymentService.WebApi.Mediatr;
using Riok.Mapperly.Abstractions;
using System.Numerics;

namespace PaymentService.WebApi.Common.Mappers
{
    /// <summary>
    /// Маппер объектов платежей
    /// </summary>
    [Mapper]
    public partial class PaymentMapper
    {
        /// <summary>
        /// Настройки типов данных
        /// </summary>
        DataTypesOptions _dataTypesOptions;
        public PaymentMapper(IOptions<DataTypesOptions> dataTypesOptions) 
        {
            _dataTypesOptions = dataTypesOptions.Value;
        }

        /// <summary>
        /// Маппер доменной модели платежа на DTO получения платежа
        /// </summary>
        /// <param name="payment">Доменная модель платежа</param>
        /// <returns>DTO получения платежа</returns>
        public partial GetPaymentDTO PaymentToGetPaymentDTO(Payment payment);

        /// <summary>
        /// Маппер DTO получения платежа на доменную модель платежа
        /// </summary>
        /// <param name="createPaymentDTO"> DTO получения платежа</param>
        /// <returns>Доменная модель платежа</returns>
        public partial Payment GetPaymentDTOToPayment(CreatePaymentDTO createPaymentDTO);

        /// <summary>
        /// Маппер команды создания платежа на DTO содания платежа
        /// </summary>
        /// <param name="createPaymentCommand">Команда создания платежа</param>
        /// <returns>DTO создания платежа</returns>
        public partial CreatePaymentDTO CreatePaymentCommandToCreatePaymentDTO(CreatePaymentCommand createPaymentCommand);

        /// <summary>
        /// Маппер доменной модели платежа на DTO обновления платежа
        /// </summary>
        /// <param name="payment">Доменная модель платежа</param>
        /// <returns>DTO обновления платежа</returns>
        public partial PaymentUpdated PaymentToPaymentUpdated(Payment payment);

        private byte[] DecimalToAvroBytes(decimal value)
        {
            decimal scaledValue = value * (decimal)Math.Pow(10, _dataTypesOptions.DecimalScale);
            BigInteger unscaled = new BigInteger(scaledValue);
            byte[] littleEndian = unscaled.ToByteArray();
            byte[] bigEndian = littleEndian.Reverse().ToArray();
            return bigEndian;
        }
        private long DateTimeToAvroLong(DateTime dateTime)
        {
            // Ensure UTC to avoid timezone issues
            var utcDateTime = dateTime.ToUniversalTime();
            return new DateTimeOffset(utcDateTime).ToUnixTimeMilliseconds();
        }
    }
}
