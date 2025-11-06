namespace InventoryService.gRPC.Domain.Entities
{
    public class InventoryItem
    {
        public string ProductId { get; private set; } = default!;
        public int Quantity { get; private set; }
        public InventoryItem(string productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public void UpdateQuantity(int qty)
        {
            Quantity = qty;
        }
        public bool HasSufficientQuantity(int requested) => Quantity >= requested;

        public void Reserve(int qty)
        {
            if (HasSufficientQuantity(qty))
                Quantity -= qty;
        }

        public void Release(int qty)
        {
            Quantity += qty;
        }
    }
}
