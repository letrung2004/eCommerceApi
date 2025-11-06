namespace ProductService.Presentation.Services.Interfaces
{
    public interface IInventoryServiceClient
    {
        public Task<bool> CreateInventoryAsync(string productId, int qty);
        public Task<bool> UpdateInventoryAsync(string productId, int qty);
    }
}
