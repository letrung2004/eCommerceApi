namespace InventoryService.gRPC.Domain.Entities
{
    public class InventoryItem
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string ProductId { get; private set; } = default!;
        public int AvailableQuantity { get; private set; }
        public int ReservedQuantity { get; private set; }
        private InventoryItem() { }

        public InventoryItem(string productId, int quantity)
        {
            ProductId = productId;
            AvailableQuantity = quantity;
            ReservedQuantity = 0;
        }

        // cập nhật số lượng
        public void UpdateQuantity(int qty)
        {
            AvailableQuantity = qty;
        }

        // trả true nếu số lượng hợp lệ (còn đủ trong kho để đặt hàng)
        public bool HasSufficientQuantity(int requested) => AvailableQuantity >= requested;

        // Reserve stock cho order (không confirm)
        public bool Reserve(int qty)
        {
            if (!HasSufficientQuantity(qty)) return false;
            AvailableQuantity -= qty;
            ReservedQuantity += qty;
            return true;
        }

        // Release stock khi cancel hoặc lỗi
        public void Release(int qty)
        {
            ReservedQuantity -= qty;
            AvailableQuantity += qty;
        }

        // Confirm stock khi order thành công
        public void Confirm()
        {
            ReservedQuantity = 0; // Đã trừ khỏi AvailableQuantity rồi
        }
    }
}
