namespace PaymentService.WebApi.Common.DTO
{
    /// <summary>
    /// DTO Получения платежа
    /// </summary>
    public class GetPaymentDTO
    {
        /// <summary>
        /// Сумма платежа
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Статус платежа, true - оплачено false - не оплачено
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Дата создания платежа
        /// </summary>
        public DateTime DateCreate { get; set; }
    }
}
