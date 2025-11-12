namespace OrderSaga.Worker.Enums
{
    public enum OrderSagaStep
    {
        NotStarted,
        OrderMarkedAsProcessing,
        InventoryReserved,
        PaymentProcessed,
        OrderCompleted
    }
}
