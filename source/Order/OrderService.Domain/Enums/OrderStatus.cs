namespace OrderService.Domain.Enums
{
    public enum OrderStatus
    {
        Pending,
        PaymentProcessing,
        PaymentCompleted,
        Shipped,
        Delivered,
        Cancelled,
        Refunded
    }
}
