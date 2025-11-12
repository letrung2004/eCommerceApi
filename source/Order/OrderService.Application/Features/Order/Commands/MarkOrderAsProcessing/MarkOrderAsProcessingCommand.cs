using MediatR;

namespace OrderService.Application.Features.Order.Commands.MarkOrderAsProcessing
{
    public class MarkOrderAsProcessingCommand : IRequest<bool>
    {
        public Guid OrderId { get; set; }
    }
}
