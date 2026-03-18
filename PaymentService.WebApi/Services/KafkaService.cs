using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PaymentService.WebApi.Common.Options;
using PaymentService.WebApi.Interfaces;
using static Confluent.Kafka.ConfigPropertyNames;

namespace PaymentService.WebApi.Services
{
    public class KafkaService : IKafkaService
    {
        ILogger _logger;
        KafkaOptions _kafkaOptions;

        public KafkaService(ILogger<KafkaService> logger,
            IOptions<KafkaOptions> kafkaOptions) 
        {
            _kafkaOptions = kafkaOptions.Value;
            _logger = logger;
        }

        public async Task ProduceAsync(string topic, string message, CancellationToken cancellationToken)
        {
            var config = new ProducerConfig()
            {
                BootstrapServers = _kafkaOptions.ServerAddress
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            try
            {
                var deliveryResult = await producer.ProduceAsync(topic, new Message<Null, string> { Value = message },cancellationToken);
            }
            catch (ProduceException<Null, string> e)
            {
                _logger.LogError($"Ошибка доставки: {e.Error.Reason}");
            }
        }
    }
}
