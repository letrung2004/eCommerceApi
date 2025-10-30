using AutoMapper;
using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces.IRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedLibrarySolution.Responses;

namespace InventoryService.Application.Features.Inventory.Queries.GetAllInventories
{
    public class GetAllInventoriesQueryHandler : IRequestHandler<GetAllInventoriesQuery, PageResponse<ProductInventoryDTO>>
    {
        private readonly IProductInventoryRepository _inventoryRepo;
        private readonly IMapper _mapper;

        public GetAllInventoriesQueryHandler(IProductInventoryRepository inventoryRepo, IMapper mapper)
        {
            _inventoryRepo = inventoryRepo;
            _mapper = mapper;
        }

        public async Task<PageResponse<ProductInventoryDTO>> Handle(GetAllInventoriesQuery request, CancellationToken cancellationToken)
        {
            var query = _inventoryRepo.Query();

            var totalItems = await query.CountAsync(cancellationToken);
            var inventories = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);


            var inventoryDtos = _mapper.Map<IEnumerable<ProductInventoryDTO>>(inventories);

            return new PageResponse<ProductInventoryDTO>(inventoryDtos, totalItems, request.PageNumber, request.PageSize);
        }
    }
}
