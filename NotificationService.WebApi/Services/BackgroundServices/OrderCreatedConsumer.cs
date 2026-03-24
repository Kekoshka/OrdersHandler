using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using NotificationService.WebApi.Common.AvroSchemas;
using NotificationService.WebApi.Common.Hubs;
using NotificationService.WebApi.Common.Mappers;
using NotificationService.WebApi.Common.Options;

namespace NotificationService.WebApi.Services.BackgroundServices
{
    /// <summary>
    /// Обрабатывает сообщения о создании заказов
    /// </summary>
    public class OrderCreatedConsumer : BackgroundService
    {

        /// <summary>
        /// Hub для отправки уведомлений через SignalR
        /// </summary>
        IHubContext<NotificationHub> _hub;

        /// <summary>
        /// Маппер объектов заказов
        /// </summary>
        OrderMapper _orderMapper;

        /// <summary>
        /// Слушатель сообщений о создании заказов
        /// </summary>
        IConsumer<string, OrderCreated> _consumer;

        /// <summary>
        /// Настройки слушателей Kafka
        /// </summary>
        KafkaConsumersOptions _kafkaConsumersOptions;

        /// <summary>
        /// Настройки внешних сервисов
        /// </summary>
        ExternalServicesOptions _externalServicesOptions;

        public OrderCreatedConsumer(
            IOptions<ExternalServicesOptions> externalServicesOptions,
            IOptions<KafkaConsumersOptions> kafkaConsumersOptions,
            ISchemaRegistryClient schemaRegistryClient,
            IHubContext<NotificationHub> hub,
            OrderMapper orderMapper) 
        {
            _hub = hub;
            _orderMapper = orderMapper;
            _kafkaConsumersOptions = kafkaConsumersOptions.Value;
            _externalServicesOptions = externalServicesOptions.Value;

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _externalServicesOptions.KafkaAddress,
                GroupId = _kafkaConsumersOptions.GroupId,
                AutoOffsetReset = Enum.Parse<AutoOffsetReset>(_kafkaConsumersOptions.AutoOffsetReset ?? "Earliest"),
                EnableAutoCommit = bool.Parse(_kafkaConsumersOptions.EnableAutoCommit ?? "true"),
            };

            var avroDeserializer = new AvroDeserializer<OrderCreated>(schemaRegistryClient).AsSyncOverAsync();
            _consumer = new ConsumerBuilder<string, OrderCreated>(consumerConfig)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(avroDeserializer)
                .Build();
        }

        /// <summary>
        /// Выполняет непрерывную обработку сообщений о создании заказов из Kafka 
        /// </summary>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_externalServicesOptions.OrderServiceTopic);

            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                if (consumeResult.Message.Value != null)
                {
                    await HandleMessageAsync(consumeResult.Message.Value, cancellationToken);
                }
            }

            _consumer.Close();
        }

        /// <summary>
        /// Отправляет уведомление клиентам о создании заказа
        /// </summary>
        /// <param name="message">Сообщение о создании заказа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns></returns>
        protected async Task HandleMessageAsync(OrderCreated message, CancellationToken cancellationToken)
        {
            var order = _orderMapper.OrderCreatedToOrder(message);
            await _hub.Clients.User(message.EmailClient).SendAsync("ShowOrderCreated", message, cancellationToken);
        }
    }
}
