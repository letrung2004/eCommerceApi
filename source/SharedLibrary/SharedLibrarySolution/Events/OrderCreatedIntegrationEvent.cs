namespace SharedLibrarySolution.Events
{
    public class OrderCreatedIntegrationEvent(Guid orderId, Guid userId, decimal totalPrice) : IntegrationEvent
    {
        public Guid OrderId { get; private set; } = orderId;
        public Guid UserId { get; private set; } = userId;
        public decimal TotalPrice { get; private set; } = totalPrice;

    }
}
