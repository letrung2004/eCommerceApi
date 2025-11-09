using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces.IRepository;
using OrderService.Infrastructure.Data;

namespace OrderService.Infrastructure.Repository
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly OrderDbContext _dbContext;
        private readonly IMapper _mapper;

        public OrderItemRepository(OrderDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<List<OrderItemResponse>> GetOrderItemsByOrderId(Guid OrderId)
        {
            var items = await _dbContext.OrderItems
                .Where(x => x.OrderId == OrderId)
                .ToListAsync(); // lấy danh sách order items
            // map từ entity sang response
            var res = _mapper.Map<List<OrderItemResponse>>(items);
            return res;
        }
    }
}
