using IdentityService.Application.DTOs;
using MediatR;
using SharedLibrarySolution.Responses;


namespace IdentityService.Application.Features.User.Queries.GetUsers
{
    public class GetUsersQuery : IRequest<PageResponse<UserDTO>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Keyword { get; set; }
    }
}
