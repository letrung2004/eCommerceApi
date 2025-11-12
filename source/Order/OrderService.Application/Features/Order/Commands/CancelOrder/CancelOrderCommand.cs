using MediatR;

namespace OrderService.Application.Features.Order.Commands.CancelOrder
{
    public class CancelOrderCommand : IRequest<bool>
    {
        public Guid OrderId { get; set; }
    }
}
