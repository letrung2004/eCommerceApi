using MediatR;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Features.Order.Commands.MarkOrderAsProcessing
{
    // Đánh dấu đơn hàng đã được tạo và đang được xử lý

    public class MarkOrderAsProcessingCommandHandler : IRequestHandler<MarkOrderAsProcessingCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MarkOrderAsProcessingCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(MarkOrderAsProcessingCommand request, CancellationToken cancellationToken)
        {
            var orderRepo = _unitOfWork.GetRepository<Domain.Entities.Order>();

            var order = await orderRepo.GetByIdAsync(request.OrderId)
                ?? throw new ArgumentException("Order not found");
            order.MarkAsProcessing();
            await orderRepo.UpdateAsync(request.OrderId, order);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
