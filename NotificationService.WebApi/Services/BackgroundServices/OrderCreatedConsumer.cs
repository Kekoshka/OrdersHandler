using Avro;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using NotificationService.WebApi.Common.AvroSchemas;
using NotificationService.WebApi.Common.DTO;
using NotificationService.WebApi.Common.Hubs;
using NotificationService.WebApi.Common.Mappers;
using NotificationService.WebApi.Common.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace NotificationService.WebApi.Services.BackgroundServices
{
    /// <summary>
    /// Обрабатывает сообщения о создании заказов
    /// </summary>
    public class OrderCreatedConsumer : ConsumerBase<OrderCreated>
    {
        /// <summary>
        /// Маппер объектов заказов
        /// </summary>
        OrderMapper _orderMapper;

        public OrderCreatedConsumer(IOptions<ExternalServicesOptions> externalServicesOptions,
            IOptions<KafkaConsumersOptions> kafkaConsumersOptions,
            ISchemaRegistryClient schemaRegistryClient, 
            IHubContext<NotificationHub> hub, 
            ILogger<OrderCreatedConsumer> logger,
            OrderMapper orderMapper) 
            : base(externalServicesOptions, 
                  kafkaConsumersOptions, 
                  schemaRegistryClient, 
                  hub, 
                  logger)
        {
            _orderMapper = orderMapper;
        }

        /// <summary>
        /// Выполняет непрерывную обработку сообщений о создании заказов из Kafka 
        /// </summary>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_externalServicesOptions.OrderServiceTopic);

            await Task.Run(async ()  =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    var message = consumeResult.Message.Value;
                    if (message is not null)
                    {
                        var order = _orderMapper.OrderCreatedToOrder(message);
                        var jsonOrder = JsonSerializer.Serialize(order);
                        await _hub.Clients.User(message.EmailClient).SendAsync("ShowOrderCreated", jsonOrder, cancellationToken);
                    }
                }
            });

            _consumer.Close();
        }
    }
}
