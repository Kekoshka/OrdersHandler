using Confluent.Kafka;
using Microsoft.Extensions.Options;
using OrderService.WebApi.Common.Options;
using OrderService.WebApi.Interfaces;
using System.Threading;

namespace OrderService.WebApi.Services
{
    public class KafkaService:IKafkaService
    {

        KafkaOptions _kafkaOptions;
        ILogger _logger;
        public KafkaService(ILogger<KafkaService> logger,
            IOptions<KafkaOptions> kafkaOptions)
        {
            _logger = logger;
            _kafkaOptions = kafkaOptions.Value;
        }
        public async Task Produce(string topic, string message,CancellationToken cancellationToken)
        {
            var config = new ProducerConfig()
            {
                BootstrapServers = _kafkaOptions.ServerAddress
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            try
            {
                var deliveryResult = await producer.ProduceAsync(topic, new Message<Null, string> { Value = message }, cancellationToken);
            }
            catch (ProduceException<Null, string> e)
            {
                _logger.LogError($"Ошибка доставки: {e.Error.Reason}");
            }
        }
    }
}
