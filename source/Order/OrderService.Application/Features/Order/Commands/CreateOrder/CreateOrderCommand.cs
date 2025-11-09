using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Order.Commands.CreateOrder
{
    // đầu vào
    public class CreateOrderCommand : IRequest<OrderResponse>
    {
        public Guid UserId { get; set; } // user tạo đơn hàng
        public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>(); // danh sách các order items
    }
}
