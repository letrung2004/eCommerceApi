namespace ProductService.Presentation.Entities.Events
{
    public class ProductUpdatedEvent
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
