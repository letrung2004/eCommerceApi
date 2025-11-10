using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces.IRepository;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Data;
using SharedLibrarySolution.Commons;
using SharedLibrarySolution.Responses;

namespace OrderService.Infrastructure.Persistence.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _dbContext;
        private readonly IMapper _mapper;
        public OrderRepository(OrderDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<PageResponse<OrderResponse>> GetPaginatedOrders(Specification<Order> specification, int page, int pageSize)
        {
            var query = _dbContext.Orders.Where(specification.Criteria); // lấy danh sách có truyền query 
            int total = await query.CountAsync(); // đếm số phần tử để phân trang

            var orders = await query
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();
            var orderResponse = _mapper.Map<List<OrderResponse>>(orders);
            return new PageResponse<OrderResponse>(orderResponse, total, page, pageSize);
        }
    }
} 
