namespace SharedLibrarySolution.Events
{
    public class IntegrationEvent
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;

    }
}
