namespace OrderSaga.Worker.DTOs
{
    public class PaymentResult
    {
        public bool IsSuccessful { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string FailureReason { get; set; } = string.Empty;
    }
}
