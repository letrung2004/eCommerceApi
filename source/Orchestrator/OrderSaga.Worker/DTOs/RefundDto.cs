namespace OrderSaga.Worker.DTOs
{
    public class RefundDto
    {
        public string Id { get; set; } = String.Empty;
        public string OrderId { get;  set; } = String.Empty;
        public decimal Amount { get;  set; }
        public string Status { get;  set; } = string.Empty;
        public DateTime RequestedAt { get;  set; } 
    }
}
