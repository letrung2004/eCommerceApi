using InventoryService.Application.DTOs;
using MediatR;
using SharedLibrarySolution.Responses;

namespace InventoryService.Application.Features.Inventory.Queries.GetAllInventories
{
    public class GetAllInventoriesQuery : IRequest<PageResponse<ProductInventoryDTO>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
