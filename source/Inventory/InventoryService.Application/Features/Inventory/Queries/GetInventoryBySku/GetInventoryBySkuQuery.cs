using InventoryService.Application.DTOs;
using MediatR;

namespace InventoryService.Application.Features.Inventory.Queries.GetInventoryBySku
{
    public class GetInventoryBySkuQuery : IRequest<ProductInventoryDTO?>
    {
        public string Sku { get; set; } = string.Empty;
    }
}
