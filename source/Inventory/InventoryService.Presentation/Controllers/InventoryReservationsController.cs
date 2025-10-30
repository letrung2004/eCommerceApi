using InventoryService.Application.DTOs;
using InventoryService.Application.Features.Reservations.Queries.GetAllReservations;
using InventoryService.Application.Features.Reservations.Queries.GetReservationById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrarySolution.Responses;

namespace InventoryService.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryReservationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoryReservationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllInventoryReservationsQuery());
            return Ok(new ApiResponse<IEnumerable<InventoryReservationDTO>>(200, result));
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrderId(Guid orderId)
        {
            var result = await _mediator.Send(new GetReservationsByOrderIdQuery { OrderId = orderId });
            return Ok(new ApiResponse<IEnumerable<InventoryReservationDTO>>(200, result));
        }
    }
}
