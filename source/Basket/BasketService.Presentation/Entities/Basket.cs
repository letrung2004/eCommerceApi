namespace BasketService.Presentation.Entities
{
    public class Basket
    {
        public Guid UserId { get; set; }
        public List<BasketItem> Items { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Thời gian sống để Redis auto xóa
        public TimeSpan TTL => TimeSpan.FromHours(12); // ✅ Đổi thành property

        // ✅ Tổng giá tiền
        public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
    }
}
