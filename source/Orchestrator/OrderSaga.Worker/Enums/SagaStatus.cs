namespace OrderSaga.Worker.Enums
{
    public enum SagaStatus
    {
        Started,
        Completed,
        Failed,
        CompensationFailed
    }
}
