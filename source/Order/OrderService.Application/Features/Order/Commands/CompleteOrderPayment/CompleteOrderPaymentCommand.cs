using MediatR;

namespace OrderService.Application.Features.Order.Commands.CompleteOrderPayment
{
    public class CompleteOrderPaymentCommand : IRequest<bool>
    {
        public Guid OrderId { get; set; }
    }
}
