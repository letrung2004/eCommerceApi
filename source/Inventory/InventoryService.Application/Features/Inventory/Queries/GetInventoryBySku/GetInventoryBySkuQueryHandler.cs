using AutoMapper;
using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces.IRepositories;
using MediatR;

namespace InventoryService.Application.Features.Inventory.Queries.GetInventoryBySku
{
    public class GetInventoryBySkuQueryHandler : IRequestHandler<GetInventoryBySkuQuery, ProductInventoryDTO?>
    {
        private readonly IProductInventoryRepository _inventoryRepo;
        private readonly IMapper _mapper;

        public GetInventoryBySkuQueryHandler(IProductInventoryRepository inventoryRepo, IMapper mapper)
        {
            _inventoryRepo = inventoryRepo;
            _mapper = mapper;
        }

        public async Task<ProductInventoryDTO?> Handle(GetInventoryBySkuQuery request, CancellationToken cancellationToken)
        {
            var inventory = await _inventoryRepo.GetBySkuAsync(request.Sku);
            return _mapper.Map<ProductInventoryDTO?>(inventory);
        }
    }
}
