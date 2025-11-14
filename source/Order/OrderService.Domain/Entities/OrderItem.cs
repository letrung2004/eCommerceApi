namespace OrderService.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid OrderId { get; private set; }         // Liên kết với Order
        public string ProductId { get; private set; } = ""; // ID sản phẩm
        public string Sku { get; set; } = string.Empty; // update db
        public int Quantity { get; private set; }          // Số lượng
        public decimal Price { get; private set; }         // Giá 1 sản phẩm

        // Computed property: tự tính tổng giá trị sản phẩm
        public decimal TotalPrice => Quantity * Price;

        // Constructor chính — tạo khi thêm sản phẩm vào đơn
        public OrderItem(Guid orderId, string productId, int quantity, decimal price, string sku)
        {
            if (quantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0");
            if (price <= 0)
                throw new ArgumentException("Giá phải lớn hơn 0");

            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
            Sku = sku;
        }

        // EF Core cần constructor rỗng để mapping DB
        private OrderItem() { }

        // Có thể thêm hành vi nếu muốn sau này
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0");

            Quantity = newQuantity;
        }
    }
}

// update database