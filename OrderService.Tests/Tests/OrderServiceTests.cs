using ExceptionHandler.Exceptions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using OrderService.DataAccess.Postgres.Context;
using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.Common.DTO;
using OrderService.WebApi.Common.ExternalApi;
using OrderService.WebApi.Common.Mappers;
using OrderService.WebApi.Common.Options;
using OrderService.WebApi.Interfaces;
using PaymentService.WebApi.Common.Options;
using SQLitePCL;

namespace OrderService.Tests.Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private AppDbContext _context;
        private SqliteConnection _connection;
        private OrderMapper _orderMapper;
        private Mock<IPaymentServiceApi> _paymentServiceApiMock;
        private Mock<IKafkaService> _kafkaServiceMock;
        private Mock<IOptions<ExternalServicesOptions>> _externalOptionsMock;
        private Mock<IOptions<DataTypesOptions>> _dataTypeOptionsMock;
        private WebApi.Services.OrderService _orderService;

        [SetUp]
        public void SetUp()
        {
            // Используем V2 Init для современных бандлов
            Batteries_V2.Init();

            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new AppDbContext(options);

            _context.Database.EnsureCreated();

            _dataTypeOptionsMock = new Mock<IOptions<DataTypesOptions>>();
            _dataTypeOptionsMock.Setup(x => x.Value).Returns(new DataTypesOptions
            {
                DecimalScale = 10,
                DecimalPrecision = 2
            });

            _orderMapper = new OrderMapper(_dataTypeOptionsMock.Object);

            _paymentServiceApiMock = new Mock<IPaymentServiceApi>();
            _kafkaServiceMock = new Mock<IKafkaService>();

            _externalOptionsMock = new Mock<IOptions<ExternalServicesOptions>>();
            _externalOptionsMock.Setup(x => x.Value).Returns(new ExternalServicesOptions
            {
                OrderServiceTopic = "order-service-test"
            });

            _orderService = new WebApi.Services.OrderService(
                _context,
                _orderMapper,
                _paymentServiceApiMock.Object,
                _kafkaServiceMock.Object,
                _externalOptionsMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                if (_context != null)
                {
                    _context.Database.EnsureDeleted();
                    _context.Dispose();
                    _context = null;
                }
            }
            finally
            {
                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }

        [Test]
        public async Task CreateOrderAsync_ShouldCreateOrder_AndCallKafka_AndCallPaymentApi()
        {
            var orderDto = new OrderDTO
            {
                ProductId = 1,
                EmailClient = "test@test.test",
                Amount = 10,
                PhoneNumber = "89504444444",
                Price = 100.50m
            };

            var orderId = await _orderService.CreateOrderAsync(orderDto, CancellationToken.None);

            var createdOrder = await _context.Orders.FindAsync(orderId);
            Assert.That(createdOrder, Is.Not.Null);
            Assert.That(createdOrder.ProductId, Is.EqualTo(1));
            Assert.That(createdOrder.EmailClient, Is.EqualTo("test@test.test"));
            Assert.That(createdOrder.Amount, Is.EqualTo(10));
            Assert.That(createdOrder.PhoneNumber, Is.EqualTo("89504444444"));
            Assert.That(createdOrder.Price, Is.EqualTo(100.50m));

            _kafkaServiceMock.Verify(
                x => x.ProduceAsync(
                    _externalOptionsMock.Object.Value.OrderServiceTopic,
                    It.IsAny<OrderCreated>(), 
                    orderId,
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );

            _paymentServiceApiMock.Verify(
                x => x.CreatePaymentAsync(
                    It.Is<CreatePaymentRequestDTO>(dto => dto.OrderId == orderId && dto.Price == 100.50m),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task GetOrderAsync_ShouldReturnOrderDTO_WhenOrderExists()
        {
            var order = new Order
            {
                ProductId = 1,
                Price = 100.50m,
                EmailClient = "test@test.test",
                Amount = 2,
                PhoneNumber = "89504444444"
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var result = await _orderService.GetOrderAsync(1, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProductId, Is.EqualTo(order.Id));
            Assert.That(result.Price, Is.EqualTo(100.50m));
            Assert.That(result.EmailClient, Is.EqualTo("test@test.test"));
            Assert.That(result.Amount, Is.EqualTo(2));
            Assert.That(result.PhoneNumber, Is.EqualTo("89504444444"));
        }

        [Test]
        public void GetOrderAsync_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
        {
            var nonExistedId = 999;
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _orderService.GetOrderAsync(nonExistedId, CancellationToken.None));
        }

        [Test]
        public async Task DeleteOrderAsync_ShouldRemoveOrder_WhenOrderExists()
        {
            var order = new Order
            {
                ProductId = 1,
                Price = 100.50m,
                EmailClient = "test@test.test",
                Amount = 2,
                PhoneNumber = "89504444444"
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await _orderService.DeleteOrderAsync(order.Id, CancellationToken.None);

            var deletedOrder = await _context.Orders.FindAsync(order.Id);
            Assert.That(deletedOrder, Is.Null);
        }

        [Test]
        public void DeleteOrderAsync_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
        {
            var nonExistedId = 999;
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _orderService.DeleteOrderAsync(nonExistedId, CancellationToken.None));
        }
    }
}
