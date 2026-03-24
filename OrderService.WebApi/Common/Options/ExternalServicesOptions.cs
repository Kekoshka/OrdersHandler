namespace OrderService.WebApi.Common.Options
{
    /// <summary>
    /// Настройки внешних сервисов
    /// </summary>
    public class ExternalServicesOptions
    {
        /// <summary>
        /// Адрес Kafka
        /// </summary>
        public string KafkaAddress { get; set; }

        /// <summary>
        /// Адрес schema registry
        /// </summary>
        public string SchemaRegistryAddress { get; set; }

        /// <summary>
        /// Название топика в Kafka для сервиса заказов
        /// </summary>
        public string OrderServiceTopic { get; set; }

        /// <summary>
        /// Название топика в Kafka для сервиса платежей
        /// </summary>
        public string PaymentServiceTopic { get; set; }
    }
}
