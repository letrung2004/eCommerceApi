using OrderService.Domain.Entities;
using SharedLibrarySolution.Interfaces;

namespace OrderService.Application.DTOs
{
    public class OrderItemResponse : IMapFrom<OrderItem>
    {
        public required string ProductId { get; set; }
        public required string Sku { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
