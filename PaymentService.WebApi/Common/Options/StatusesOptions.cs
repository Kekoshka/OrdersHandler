namespace PaymentService.WebApi.Common.Options
{
    /// <summary>
    /// Настройки статусов платежа
    /// </summary>
    public class StatusesOptions
    {
        /// <summary>
        /// В процессе
        /// </summary>
        public int InProgress { get; set; }
        
        /// <summary>
        /// Выполнен
        /// </summary>
        public int IsCompleted { get; set; }
    }
}
