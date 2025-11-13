using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs;
using OrderService.Application.Features.Order.Commands.CancelOrder;
using OrderService.Application.Features.Order.Commands.CompleteOrderPayment;
using OrderService.Application.Features.Order.Commands.CreateOrder;
using OrderService.Application.Features.Order.Commands.MarkOrderAsProcessing;
using OrderService.Application.Features.Order.Queries.OrderDetailById.GetOrderItemById;
using SharedLibrarySolution.Responses;

namespace OrderService.Presentation.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Tạo mới đơn hàng</summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string>(
                    StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ"));
            try
            {
                var result = await _mediator.Send(command);
                return Ok(new ApiResponse<OrderResponse>(
                    StatusCodes.Status200OK, "Tạo đơn hàng thành công", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<string>(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>Xác nhận thanh toán đơn hàng</summary>
        [HttpPut("{orderId}/mark-paid")]
        public async Task<IActionResult> MarkAsPaid([FromRoute] Guid orderId)
        {
            try
            {
                var command = new CompleteOrderPaymentCommand { OrderId = orderId };
                await _mediator.Send(command);

                return Ok(new ApiResponse<string>(
                    StatusCodes.Status200OK, "Mark as paid success"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<string>(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>Đánh dấu đơn hàng đang xử lý</summary>
        [HttpPut("{orderId}/mark-processing")]
        public async Task<IActionResult> MarkAsProcessing([FromRoute] Guid orderId)
        {
            try
            {
                var command = new MarkOrderAsProcessingCommand { OrderId = orderId };
                await _mediator.Send(command);

                return Ok(new ApiResponse<string>(
                    StatusCodes.Status200OK, "Mark as processing success"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<string>(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>Hủy đơn hàng</summary>
        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder([FromRoute] Guid orderId)
        {
            try
            {
                var command = new CancelOrderCommand { OrderId = orderId };
                await _mediator.Send(command);

                return Ok(new ApiResponse<string>(
                    StatusCodes.Status200OK, "Cancel order success"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<string>(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        /// <summary>Lấy chi tiết đơn hàng và danh sách sản phẩm trong đơn</summary>
        [HttpGet("{orderId}/items")]
        public async Task<IActionResult> GetOrderItemsByOrderId([FromRoute] Guid orderId)
        {
            try
            {
                var query = new GetOrderItemByIdQuery { OrderId = orderId };
                var result = await _mediator.Send(query);

                return Ok(new ApiResponse<List<OrderItemResponse>>(
                    StatusCodes.Status200OK, "Lấy chi tiết đơn hàng thành công", result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<string>(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
