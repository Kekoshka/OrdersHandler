namespace PaymentService.WebApi.Interfaces
{
    public interface IKafkaService
    {
        public Task ProduceAsync<TKey, TValue>(string topic, TValue value, TKey key, CancellationToken cancellationToken);
    }
}
