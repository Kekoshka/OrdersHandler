namespace OrderService.WebApi.Interfaces
{
    /// <summary>
    /// Определяет контракты для сервиса работы с Kafka
    /// </summary>
    public interface IKafkaService
    {
        /// <summary>
        /// Отправить сообщение в Kafka
        /// </summary>
        /// <typeparam name="TKey">Тип ключа</typeparam>
        /// <typeparam name="TValue">Тип сообщения</typeparam>
        /// <param name="topic">Топик</param>
        /// <param name="value">Сообщение</param>
        /// <param name="key">Ключ</param>
        /// <param name="cancellationToken">Токен отмены</param>
        public Task ProduceAsync<TKey, TValue>(string topic, TValue value, TKey key, CancellationToken cancellationToken);

    }
}
