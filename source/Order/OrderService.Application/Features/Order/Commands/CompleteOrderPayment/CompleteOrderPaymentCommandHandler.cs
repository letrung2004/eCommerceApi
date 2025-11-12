using MediatR;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Features.Order.Commands.CompleteOrderPayment
{
    public class CompleteOrderPaymentCommandHandler : IRequestHandler<CompleteOrderPaymentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompleteOrderPaymentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(CompleteOrderPaymentCommand request, CancellationToken cancellationToken)
        {
            var orderRepo = _unitOfWork.GetRepository<Domain.Entities.Order>();
            var order = await orderRepo.GetByIdAsync(request.OrderId)
                ?? throw new ArgumentException("Order not found");
            order.MarkAsPaid();
            await orderRepo.UpdateAsync(request.OrderId, order);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
