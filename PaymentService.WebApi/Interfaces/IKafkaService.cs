namespace PaymentService.WebApi.Interfaces
{
    public interface IKafkaService
    {
        public Task ProduceAsync(string topic, string message,CancellationToken cancellationToken);
    }
}
