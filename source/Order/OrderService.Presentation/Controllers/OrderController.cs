using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Features.Order.Commands.CreateOrder;

namespace OrderService.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IMediator mediator, ILogger<OrderController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Tạo mới một đơn hàng
        /// </summary>
        /// <param name="command">Dữ liệu đơn hàng từ client gửi lên</param>
        /// <returns>Thông tin đơn hàng sau khi tạo</returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            _logger.LogInformation("📦 Nhận request tạo đơn hàng cho UserId: {UserId}", command.UserId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("❌ Request không hợp lệ: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _mediator.Send(command);

                _logger.LogInformation("✅ Đã tạo đơn hàng thành công, Id: {OrderId}", result.Id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Lỗi khi tạo đơn hàng");
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
