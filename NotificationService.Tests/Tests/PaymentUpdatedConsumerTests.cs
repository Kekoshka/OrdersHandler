using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NotificationService.WebApi.Common.AvroSchemas;
using NotificationService.WebApi.Common.DTO;
using NotificationService.WebApi.Common.ExternalApi;
using NotificationService.WebApi.Common.Hubs;
using NotificationService.WebApi.Common.Mappers;
using NotificationService.WebApi.Common.Options;
using NotificationService.WebApi.Services.BackgroundServices;
using System.Numerics;
using System.Text.Json;
using static Confluent.Kafka.ConfigPropertyNames;

namespace NotificationService.Tests.Tests
{
    public class PaymentUpdatedConsumerTests
    {
        private IContainer _kafkaContainer;
        private IContainer _schemaRegistryContainer;
        private string _kafkaBootstrapServers;
        private string _schemaRegistryAddress;
        private readonly string _testTopicName = "payment-updated-integration-topic";
        private readonly string _clientEmail = "test@test.test";
        private readonly long _orderId = 1;

        private Mock<IHubContext<NotificationHub>> _hubContextMock;
        private Mock<IHubClients> _hubClientsMock;
        private Mock<IClientProxy> _clientProxyMock;
        private Mock<IOptions<DataTypesOptions>> _dataTypeOptionsMock;
        private PaymentMapper _paymentMapper;
        private PaymentUpdatedConsumer _consumer;
        private Mock<ILogger<PaymentUpdatedConsumer>> _loggerMock;
        private string _capturedJson;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var network = new NetworkBuilder()
                .WithName(Guid.NewGuid().ToString())
                .Build();
            await network.CreateAsync();
            string kafkaName = Guid.NewGuid().ToString();
            string schemaRegistryName = Guid.NewGuid().ToString();
            int randomPort = new Random().Next(49152, 50000);

            _kafkaContainer = new ContainerBuilder("apache/kafka:4.2.0")
                .WithExposedPort(randomPort)
                .WithPortBinding(randomPort, randomPort)
                .WithNetwork(network)
                .WithName(kafkaName)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(".*Kafka Server started.*"))
                .WithEnvironment("KAFKA_PROCESS_ROLES", "broker,controller")
                .WithEnvironment("KAFKA_NODE_ID", "1")
                .WithEnvironment("KAFKA_CONTROLLER_QUORUM_VOTERS", $"1@{kafkaName}:9093")
                .WithEnvironment("KAFKA_LISTENERS", $"INTERNAL://0.0.0.0:29092,EXTERNAL://0.0.0.0:{randomPort},CONTROLLER://0.0.0.0:9093")
                .WithEnvironment("KAFKA_ADVERTISED_LISTENERS", $"INTERNAL://{kafkaName}:29092,EXTERNAL://localhost:{randomPort},CONTROLLER://{kafkaName}:9093")
                .WithEnvironment("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "INTERNAL:PLAINTEXT,EXTERNAL:PLAINTEXT,CONTROLLER:PLAINTEXT")
                .WithEnvironment("KAFKA_INTER_BROKER_LISTENER_NAME", "INTERNAL")
                .WithEnvironment("KAFKA_CONTROLLER_LISTENER_NAMES", "CONTROLLER")
                .WithEnvironment("KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "1")
                .WithEnvironment("KAFKA_GROUP_INITIAL_REBALANCE_DELAY_MS", "0")
                .WithEnvironment("CLUSTER_ID", "MkU3OEVBNTcwNTJENDM2Qk")
                .Build();

            _schemaRegistryContainer = new ContainerBuilder("confluentinc/cp-schema-registry:8.0.0")
                .WithExposedPort(8081)
                .WithPortBinding(8081, true)
                .WithNetwork(network)
                .WithName(schemaRegistryName)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(".*Server started, listening for requests.*"))
                .WithEnvironment("SCHEMA_REGISTRY_HOST_NAME", schemaRegistryName)
                .WithEnvironment("SCHEMA_REGISTRY_KAFKASTORE_BOOTSTRAP_SERVERS", $"PLAINTEXT://{kafkaName}:29092")
                .WithEnvironment("SCHEMA_REGISTRY_LISTENERS", "http://0.0.0.0:8081")
                .Build();

            await _kafkaContainer.StartAsync();
            await _schemaRegistryContainer.StartAsync();

            _schemaRegistryAddress = "http://localhost:" + _schemaRegistryContainer.GetMappedPublicPort(8081).ToString();
            _kafkaBootstrapServers = $"localhost:{randomPort}";

        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            if (_schemaRegistryContainer != null) await _schemaRegistryContainer.DisposeAsync();
            if (_kafkaContainer != null) await _kafkaContainer.DisposeAsync();
        }

        [SetUp]
        public void SetUp()
        {
            _dataTypeOptionsMock = new Mock<IOptions<DataTypesOptions>>();
            _dataTypeOptionsMock.Setup(x => x.Value).Returns(new DataTypesOptions { DecimalScale = 10, DecimalPrecision = 2 });
            _paymentMapper = new PaymentMapper(_dataTypeOptionsMock.Object);

            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
            _hubClientsMock = new Mock<IHubClients>();
            _clientProxyMock = new Mock<IClientProxy>();
            _hubContextMock.Setup(h => h.Clients).Returns(_hubClientsMock.Object);
            _hubClientsMock.Setup(c => c.User(_clientEmail)).Returns(_clientProxyMock.Object);

            _capturedJson = null;
            _clientProxyMock.Setup(p => p.SendCoreAsync("ShowPaymentUpdated", It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Callback<string, object[], CancellationToken>((method, json, ct) => _capturedJson = (string)json.First())
                .Returns(Task.CompletedTask);

            var externalOptions = Options.Create(new ExternalServicesOptions
            {
                KafkaAddress = _kafkaBootstrapServers,
                SchemaRegistryAddress = _schemaRegistryAddress,
                PaymentServiceTopic = _testTopicName
            });

            var kafkaOptions = Options.Create(new KafkaConsumersOptions
            {
                GroupId = "integration-test-group",
                AutoOffsetReset = "Earliest",
                EnableAutoCommit = "true"
            });

            var schemaRegistryClient = new CachedSchemaRegistryClient(new SchemaRegistryConfig { Url = _schemaRegistryAddress });

            _loggerMock = new Mock<ILogger<PaymentUpdatedConsumer>>();
            Mock<IOrderServiceApi> orderServiceApiMock = new();
            orderServiceApiMock.Setup(x => x.GetOrderAsync(_orderId, It.IsAny<CancellationToken>())).ReturnsAsync(new GetOrderRequestDTO
            {
                ProductId = 1,
                Amount = 2,
                EmailClient = _clientEmail,
                PhoneNumber = "89504444444",
                Price = 200.50m
            });

            _consumer = new PaymentUpdatedConsumer(
                externalOptions,
                kafkaOptions,
                schemaRegistryClient,
                _hubContextMock.Object,
                _loggerMock.Object,
                _paymentMapper,
                orderServiceApiMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _consumer?.Dispose();
        }

        [Test]
        public async Task ConsumerReceivesMessage_SendsSignalRNotification()
        {
            using var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig { Url = _schemaRegistryAddress });

            var producerConfig = new ProducerConfig { BootstrapServers = _kafkaBootstrapServers };

            PaymentUpdated paymentUpdated = new()
            {
                DateCreate = DateTimeToAvroLong(DateTime.UtcNow),
                Status = false,
                Id = 1,
                OrderId = _orderId,
                Price = DecimalToAvroBytes(200.50m)
            };

            using (var producer = new ProducerBuilder<string, PaymentUpdated>(producerConfig)
                .SetValueSerializer(new AvroSerializer<PaymentUpdated>(schemaRegistry))
                .Build())
            {
                var msg = new Message<string, PaymentUpdated> { Key = paymentUpdated.Id.ToString(), Value = paymentUpdated };
                var res = await producer.ProduceAsync(_testTopicName, msg);
                Assert.That(res.Status, Is.EqualTo(PersistenceStatus.Persisted));
            }

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            var consumerTask = Task.Run(() => _consumer.StartAsync(cts.Token), cts.Token);

            await Task.Delay(3000, CancellationToken.None);

            cts.Cancel();

            try { await consumerTask; } catch (OperationCanceledException) { }

            _clientProxyMock.Verify(p => p.SendCoreAsync("ShowPaymentUpdated", It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotNull(_capturedJson, "Ожидалось, что consumer отправит сообщение через SignalR");
            var deserialized = JsonSerializer.Deserialize<Payment>(_capturedJson);
            Assert.NotNull(deserialized);
            Assert.That(deserialized.Id, Is.EqualTo(paymentUpdated.Id));
            Assert.That(deserialized.Price, Is.EqualTo(200.50m));
            Assert.That(deserialized.Status, Is.EqualTo(paymentUpdated.Status));
            Assert.That(deserialized.OrderId, Is.EqualTo(paymentUpdated.OrderId));
            Assert.That(deserialized.DateCreate, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));


        }

        private byte[] DecimalToAvroBytes(decimal value)
        {
            decimal scaled = value * (decimal)Math.Pow(10, _dataTypeOptionsMock.Object.Value.DecimalScale);
            var unscaled = new BigInteger(scaled);
            var little = unscaled.ToByteArray();
            var big = little.Reverse().ToArray();
            return big;
        }
        private long DateTimeToAvroLong(DateTime dateTime)
        {
            var utcDateTime = dateTime.ToUniversalTime();
            return new DateTimeOffset(utcDateTime).ToUnixTimeMilliseconds();
        }
    }
}
