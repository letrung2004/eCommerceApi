using Azure.Core;
using MediatR;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Persistence;
using SharedLibrarySolution.Interfaces;

namespace OrderService.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OrderDbContext _context;

        private readonly Dictionary<Type, object> _repositories = new(); // repo type cho các class khác nhau
        public UnitOfWork(OrderDbContext context)
        {
            _context = context;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        //1 UnitOfWork cho tất cả entity
        public IGenericInterface<T> GetRepository<T>() where T : class
        {
            // kiểm tra có entity T chưa
            if (!_repositories.ContainsKey(typeof(T)))
            {
                _repositories[typeof(T)] = new GenericRepository<T>(_context);
            }
            return (IGenericInterface<T>)_repositories[typeof(T)];
        }
    }
}

//Unit of Work pattern

//UnitOfWork đảm bảo rằng tất cả các thay đổi được lưu (commit) trong một giao dịch duy nhất (transaction).


//nếu lưu theo cách cũ
//await _orderRepository.CreateAsync(order);
//await _orderItemRepository.CreateAsync(item);
//Nếu có 2 repository (Order và OrderItem) mà OrderItem bị lỗi khi thêm thì order vẫn lưu


// cách mới
//var orderRepo = _unitOfWork.GetRepository<Order>();
//var newOrder = new Order(Guid.NewGuid(), request.UserId, new List<OrderItem>());

//await orderRepo.CreateAsync(newOrder);
//await _unitOfWork.CommitAsync(); -> Commit transaction do đó repo chỉ đc tra về đôi tường chứ khoan hãy commit
