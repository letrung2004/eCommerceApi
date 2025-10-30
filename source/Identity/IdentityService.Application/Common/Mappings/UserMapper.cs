using IdentityService.Application.Features.User.Commands.CreateUser;
using IdentityService.Application.Features.User.Commands.UpdateUser;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Common.Mappings
{
    public static class UserMapper
    {
        public static void MapFromCreateCommand(this User user, CreateUserCommand request)
        {
            user.UserName = request.UserName;
            user.FullName = request.FullName;
            user.PasswordHash = request.Password;
            user.Email = request.Email;
            user.Phone = request.Phone;
            user.Address = request.Address;
        }
        public static void MapFromUpdateCommand(this User user, UpdateUserCommand request)
        {
            if (!string.IsNullOrWhiteSpace(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrWhiteSpace(request.Phone))
                user.Phone = request.Phone;

            if (!string.IsNullOrWhiteSpace(request.Address))
                user.Address = request.Address;

            // Chỉ băm và cập nhật nếu có password mới
            if (!string.IsNullOrWhiteSpace(request.Password))
                user.PasswordHash = request.Password;
        }
    }
}
