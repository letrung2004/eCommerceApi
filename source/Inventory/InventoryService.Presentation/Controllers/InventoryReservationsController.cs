using InventoryService.Application.DTOs;
using InventoryService.Application.Features.Reservations.Commands.CompleteReservation;
using InventoryService.Application.Features.Reservations.Commands.CreateReservation;
using InventoryService.Application.Features.Reservations.Commands.ReleaseReservation;
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

        // khi khách hàng tạo đơn hàng gọi đến để giữ hàng -> giảm số lượng trong kho để tránh không bị người khác mua mất
        [HttpPost("reserve")]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new ApiResponse<InventoryReservationDTO>(201, result));
        }

        // đơn hàng bị hủy hoặc hết hạn -> giải phóng trả về đúng số lượng sản phẩm cho kho
        [HttpPost("release")]
        public async Task<IActionResult> ReleaseReservation([FromBody] ReleaseReservationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new ApiResponse<InventoryReservationDTO>(200, result));
        }
        // hoàn thành đơn hàn thanh toán thành công gọi api này để cập nhật status
        [HttpPost("complete")]
        public async Task<IActionResult> CompleteReservation([FromBody] CompleteReservationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new ApiResponse<InventoryReservationDTO>(200, result));
        }

    }
}
