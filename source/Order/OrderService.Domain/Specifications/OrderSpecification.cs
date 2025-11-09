using OrderService.Domain.Entities;
using SharedLibrarySolution.Commons;

// kế thừa specifications để có thể viết các filter 
namespace OrderService.Domain.Specifications
{
    public class OrderSpecification : Specification<Order>
    {
        public OrderSpecification(DateTime? createTo, DateTime? createFrom)
        {
            Criteria = order =>
            (!createFrom.HasValue || order.CreatedAt >= createFrom.Value) &&
            (!createTo.HasValue || order.CreatedAt <= createTo.Value);
        }
    }
}
