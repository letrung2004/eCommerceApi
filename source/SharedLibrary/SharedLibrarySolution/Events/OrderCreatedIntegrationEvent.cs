namespace SharedLibrarySolution.Events
{
    public class OrderCreatedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; } 
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; }

        public OrderCreatedIntegrationEvent() { } // Bắt buộc

        public OrderCreatedIntegrationEvent(Guid orderId, Guid userId, decimal totalPrice)
        {
            OrderId = orderId;
            UserId = userId;
            TotalPrice = totalPrice;
        }
    }
}
