using ExceptionHandler.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Options;
using Moq;
using PaymentService.DataAccess.Postgres.Context;
using PaymentService.DataAccess.Postgres.Models;
using PaymentService.WebApi.Common.DTO;
using PaymentService.WebApi.Common.Mappers;
using PaymentService.WebApi.Common.Options;
using PaymentService.WebApi.Interfaces;

namespace PaymentService.Tests.Tests
{

    [TestFixture]
    public class PaymentServiceTests
    {
        Mock<IOptions<ExternalServicesOptions>> _externalServicesOptionsMock;
        Mock<IOptions<StatusesOptions>> _statusesOptionsMock;
        Mock<IOptions<DataTypesOptions>> _dataTypesOptionsMock;
        Mock<IKafkaService> _kafkaServiceMock;
        PaymentMapper _paymentMapper;
        AppDbContext _context;
        WebApi.Services.PaymentService _paymentService;


        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);

            _externalServicesOptionsMock = new Mock<IOptions<ExternalServicesOptions>>();
            _externalServicesOptionsMock.Setup(x => x.Value).Returns(new ExternalServicesOptions
            {
                PaymentServiceTopic = "payment-service-test"
            });

            _statusesOptionsMock = new Mock<IOptions<StatusesOptions>>();

            _kafkaServiceMock = new Mock<IKafkaService>();

            _dataTypesOptionsMock = new Mock<IOptions<DataTypesOptions>>();
            _dataTypesOptionsMock.Setup(x => x.Value).Returns(new DataTypesOptions
            {
                DecimalScale = 10,
                DecimalPrecision = 38
            });

            _paymentMapper = new PaymentMapper(_dataTypesOptionsMock.Object);

            _paymentService = new WebApi.Services.PaymentService(_context,
                _paymentMapper,
                _statusesOptionsMock.Object,
                _kafkaServiceMock.Object,
                _externalServicesOptionsMock.Object);
        }

        [TearDown]
        public void TeadDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task CreatePaymentAsync_ShouldCreatePayment_WhenValidData()
        {
            var dto = new CreatePaymentDTO
            {
                OrderId = 1,
                Price = 100.50m
            };

            var paymentId = await _paymentService.CreatePaymentAsync(dto, CancellationToken.None);

            Assert.That(paymentId, Is.GreaterThan(0));

            var createdPayment = await _context.Payments.FindAsync(paymentId);
            Assert.That(createdPayment, Is.Not.Null);
            Assert.That(createdPayment.Price, Is.EqualTo(100.50m));
            Assert.That(createdPayment.OrderId, Is.EqualTo(1));
            Assert.That(createdPayment.Status, Is.EqualTo(false));
            Assert.That(createdPayment.DateCreate, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
        }

        [Test]
        public async Task UpdatePaymentAsync_ShouldUpdateStatusAndSendKafkaMessage_WhenPaymentExists()
        {
            var payment = new Payment
            {
                OrderId = 1,
                Price = 100.50m,
                Status = false,
                DateCreate = DateTime.UtcNow
            };

            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();

            await _paymentService.UpdatePaymentAsync(payment.Id, true, CancellationToken.None);

            var updatedPayment = await _context.Payments.FindAsync(payment.Id);
            Assert.That(updatedPayment, Is.Not.Null);
            Assert.That(updatedPayment.Status, Is.EqualTo(true));

            _kafkaServiceMock.Verify(x => x
                .ProduceAsync(_externalServicesOptionsMock.Object.Value.PaymentServiceTopic,
                It.IsAny<PaymentUpdated>(),
                payment.Id,
                It.IsAny<CancellationToken>()),Times.Once);
        }

        [Test]
        public void UpdatePaymentAsync_ShouldThrowNotFoundException_WhenPaymentDoesNotExist()
        {
            long nonExistentId = 999;

            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _paymentService.UpdatePaymentAsync(nonExistentId, true, CancellationToken.None));

            _kafkaServiceMock.Verify(
                x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<long>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public async Task GetPaymentAsync_ShouldReturnPaymentDTO_WhenPaymentExists()
        {
            var payment = new Payment
            {
                OrderId = 1,
                Status = true,
                Price = 100.50m,
                DateCreate = DateTime.UtcNow
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var result = await _paymentService.GetPaymentAsync(payment.Id, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo(payment.Status));
            Assert.That(result.Price, Is.EqualTo(payment.Price));
            Assert.That(result.DateCreate, Is.EqualTo(payment.DateCreate));
        }

        [Test]
        public void GetPaymentAsync_ShouldThrowNotFoundException_WhenPaymentDoesNotExist()
        {
            long nonExistentId = 999;

            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _paymentService.GetPaymentAsync(nonExistentId, CancellationToken.None));
        }

    }
}
