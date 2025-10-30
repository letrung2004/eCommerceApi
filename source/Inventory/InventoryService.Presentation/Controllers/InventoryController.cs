using InventoryService.Application.DTOs;
using InventoryService.Application.Features.Inventory.Queries.GetAllInventories;
using InventoryService.Application.Features.Inventory.Queries.GetInventoryBySku;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrarySolution.Responses;

namespace InventoryService.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: inventory
        [HttpGet]
        public async Task<IActionResult> GetAllInventories([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetAllInventoriesQuery { PageNumber = pageNumber, PageSize = pageSize });
            return Ok(new ApiResponse<PageResponse<ProductInventoryDTO>>(200, result));
        }

        // GET: inventory/{sku}
        [HttpGet("{sku}")]
        public async Task<IActionResult> GetInventoryBySku(string sku)
        {
            var result = await _mediator.Send(new GetInventoryBySkuQuery { Sku = sku });
            if (result == null)
                return NotFound(new ApiResponse<string>(404, "SKU not found"));

            return Ok(new ApiResponse<ProductInventoryDTO>(200, result));
        }
    }
}
