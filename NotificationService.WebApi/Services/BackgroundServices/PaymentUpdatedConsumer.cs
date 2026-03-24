using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using ExceptionHandler.Exceptions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using NotificationService.WebApi.Common.AvroSchemas;
using NotificationService.WebApi.Common.Hubs;
using NotificationService.WebApi.Common.Mappers;
using NotificationService.WebApi.Common.Options;
using NotificationService.WebApi.DTO;
using NotificationService.WebApi.Interfaces.ExternalApi;

namespace NotificationService.WebApi.Services.BackgroundServices
{
    /// <summary>
    /// Сервис обработки уведомлений о обновления статуса платежа из Kafka
    /// </summary>
    public class PaymentUpdatedConsumer : BackgroundService
    {

        IHubContext<NotificationHub> _hub;


        PaymentMapper _paymentMapper;


        IConsumer<string, PaymentUpdated> _consumer;


        KafkaConsumersOptions _kafkaConsumersOptions;


        ExternalServicesOptions _externalServicesOptions;


        IOrderServiceApi _orderServiceApi;

        public PaymentUpdatedConsumer(
            IOptions<ExternalServicesOptions> externalServicesOptions,
            IOptions<KafkaConsumersOptions> kafkaConsumersOptions,
            ISchemaRegistryClient schemaRegistryClient,
            IHubContext<NotificationHub> hub,
            PaymentMapper paymentMapper,
            IOrderServiceApi orderServiceApi)
        {
            _hub = hub;
            _paymentMapper = paymentMapper;
            _kafkaConsumersOptions = kafkaConsumersOptions.Value;
            _externalServicesOptions = externalServicesOptions.Value;
            _orderServiceApi = orderServiceApi;

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _externalServicesOptions.KafkaAddress,
                GroupId = _kafkaConsumersOptions.GroupId,
                AutoOffsetReset = Enum.Parse<AutoOffsetReset>(_kafkaConsumersOptions.AutoOffsetReset ?? "Earliest"),
                EnableAutoCommit = bool.Parse(_kafkaConsumersOptions.EnableAutoCommit ?? "true"),
            };

            var avroDeserializer = new AvroDeserializer<PaymentUpdated>(schemaRegistryClient).AsSyncOverAsync();
            _consumer = new ConsumerBuilder<string, PaymentUpdated>(consumerConfig)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(avroDeserializer)
                .Build();
        }

        /// <summary>
        /// Бесперерывно обрабатывает сообщения о обновлениях статуса платежа
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_externalServicesOptions.PaymentServiceTopic);

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
        /// Отправляет клиентам сообщения о обновления статуса платежа
        /// </summary>
        /// <param name="message">Сообщение о изменении статуса платежа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">Выбрасывается в случае, если заказ с id заказа, указанном в сообщении из Kafka не найден</exception>
        protected async Task HandleMessageAsync(PaymentUpdated message, CancellationToken cancellationToken)
        {
            var payment = _paymentMapper.PaymentUpdatedToPayment(message);
            var order = await _orderServiceApi.GetOrderAsync(message.OrderId, cancellationToken);
            if (order is null)
                throw new NotFoundException($"Payment update message was not sent because the order with id {message.OrderId} was not found.");
            await _hub.Clients.User(order.EmailClient).SendAsync("HandlePaymentUpdated", payment, cancellationToken);
            Console.WriteLine(order.EmailClient);
        }
    }
}
