using OrderService.Application.DTOs;

namespace OrderService.Application.Interfaces.IRepository
{
    public interface IOrderItemRepository
    {
        Task<List<OrderItemResponse>> GetOrderItemsByOrderId(Guid OrderId);
    }
}
