using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Options;
using OrderService.WebApi.Common.Options;
using OrderService.WebApi.Interfaces;

namespace OrderService.WebApi.Services
{
    /// <summary>
    /// Сервис для отправки сообщений в Kafka
    /// </summary>
    public class KafkaService:IKafkaService
    {
        /// <summary>
        /// Настройки внешних сервисов
        /// </summary>
        ExternalServicesOptions _servicesOptions;
        
        /// <summary>
        /// Логгер
        /// </summary>
        ILogger _logger;

        public KafkaService(ILogger<KafkaService> logger,
            IOptions<ExternalServicesOptions> servicesOptions)
        {
            _logger = logger;
            _servicesOptions = servicesOptions.Value;
        }

        /// <summary>
        /// Отправляет сообщения в Kafka
        /// </summary>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип сообщения</typeparam>
        /// <param name="topic">Топик</param>
        /// <param name="value">Сообщение</param>
        /// <param name="key">Ключ</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns></returns>
        public async Task ProduceAsync<TKey,TValue>(string topic,
            TValue value,
            TKey key,
            CancellationToken cancellationToken)
        {
            var producerConfig = new ProducerConfig()
            {
                BootstrapServers = _servicesOptions.KafkaAddress
            };

                var schemaRegistryConfig = new SchemaRegistryConfig()
            {
                Url = _servicesOptions.SchemaRegistryAddress
            };

                using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

                using var producer = new ProducerBuilder<TKey, TValue>(producerConfig)
                .SetValueSerializer(new AvroSerializer<TValue>(schemaRegistry))
                .Build();

                var message = new Message<TKey, TValue>()
            {
                Key = key,
                Value = value
            };

                var deliveryResult = await producer.ProduceAsync(topic, message, cancellationToken);

                if (deliveryResult.Status == PersistenceStatus.NotPersisted)
                _logger.LogWarning($"Message with offset {deliveryResult.Offset} to topic {topic} not persisted");
        }
    }
}
