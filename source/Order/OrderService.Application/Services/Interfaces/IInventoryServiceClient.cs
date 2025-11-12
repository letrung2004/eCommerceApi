namespace OrderService.Application.Services.Interfaces
{
    public interface IInventoryServiceClient
    {
        Task<bool> CheckStockAsync(string productId, int quantity);
        //Task<bool> ReserveStockAsync(string productId, int quantity);
        //Task ReleaseStockAsync(string productId, int quantity);
        //Task ConfirmStockAsync(string productId);
    }
}
