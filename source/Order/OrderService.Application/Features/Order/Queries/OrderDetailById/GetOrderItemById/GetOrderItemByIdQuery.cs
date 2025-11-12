using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Order.Queries.OrderDetailById.GetOrderItemById
{
    public class GetOrderItemByIdQuery : IRequest<List<OrderItemResponse>>
    {
        public Guid OrderId { get; set; }

    }
}
