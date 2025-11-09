using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using SharedLibrarySolution.Commons;
using SharedLibrarySolution.Responses;

namespace OrderService.Application.Interfaces.IRepository
{
    public interface IOrderRepository
    {
        Task<PageResponse<OrderResponse>> GetPaginatedOrders(Specification<Order> specification, int page, int pageSize);
    }
}
