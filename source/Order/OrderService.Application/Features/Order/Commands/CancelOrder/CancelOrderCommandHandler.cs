using MediatR;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Features.Order.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CancelOrderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var orderRepo = _unitOfWork.GetRepository<Domain.Entities.Order>();
            var order = await orderRepo.GetByIdAsync(request.OrderId)
                ?? throw new ArgumentException("Order not found");
            order.MarkAsCancelled();
            await orderRepo.UpdateAsync(request.OrderId, order);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
