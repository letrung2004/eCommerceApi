using IdentityService.Application.DTOs;
using IdentityService.Application.Features.Auth.Command.Login;
using IdentityService.Application.Features.Auth.Command.Logout;
using IdentityService.Application.Features.Auth.Command.RefreshToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrarySolution.Responses;
using System.Security.Claims;

namespace IdentityService.Presentation.Controllers
{
    [Route("auth")]
    //[Route("api/v{version:apiVersion}/auth")]
    [ApiController]
    //[ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand request)
        {

            var tokenResponse = await _mediator.Send(request);
            var response = new ApiResponse<TokenResponse>(StatusCodes.Status200OK, tokenResponse);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Lấy userId từ claim trong access token để đăng xuất
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized(new { message = "Invalid token" });

            var success = await _mediator.Send(new LogoutCommand { Id = Guid.Parse(userId) });

            if (!success)
                return BadRequest(new { message = "Logout failed" });

            var res = new ApiResponse<string>(StatusCodes.Status200OK, "Đăng xuất thành công");
            return Ok(res);
        }

        [HttpPost("refresh")]
        [AllowAnonymous] // ❗ Quan trọng: không cần access token mới gọi được
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            if (string.IsNullOrEmpty(command.RefreshToken))
            {
                return BadRequest(new ApiResponse<string>(
                    StatusCodes.Status400BadRequest,
                    "Missing refresh token"));
            }

            var tokenResponse = await _mediator.Send(command);

            var res = new ApiResponse<TokenResponse>(
                StatusCodes.Status200OK,
                "Token refreshed successfully", tokenResponse);

            return Ok(res);
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Ok(new { userId, username, role });
        }

    }
}
