using AutoMapper;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces.IRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedLibrarySolution.Responses;


namespace IdentityService.Application.Features.User.Queries.GetUsers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PageResponse<UserDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<PageResponse<UserDTO>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _userRepository.Query(); // IQueryable<User>

            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(u =>
                    u.UserName.Contains(request.Keyword) ||
                    u.Email.Contains(request.Keyword));

            var totalItems = await query.CountAsync(cancellationToken);

            var users = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var userDtos = _mapper.Map<List<UserDTO>>(users);

            return new PageResponse<UserDTO>(userDtos, totalItems, request.PageNumber, request.PageSize);
        }

        
    }
}
