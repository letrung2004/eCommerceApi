using IdentityService.Application.Interfaces.IRepositories;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedLibrarySolution.Exceptions;
using SharedLibrarySolution.Logs;
using System.Linq.Expressions;


namespace IdentityService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityDbContext _context;
        public UserRepository(IdentityDbContext context) => _context = context;

        public async Task<User> CreateAsync(User entity)
        {
            try
            {
                await _context.Users.AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw;
            }
        }

        // delete user 
        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                    throw new AppException("User not found");

                _context.Users.Remove(existingUser);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync(Expression<Func<User, bool>>? predicate = null)
        {
            try
            {
                IQueryable<User> query = _context.Users;

                if (predicate != null)
                    query = query.Where(predicate);

                return await query.Include(r => r.Role).ToListAsync();
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw;
            }
        }

        // Get user by condition (VD: username, email)
        //public async Task<User?> GetByAsync(Expression<Func<User, bool>> predicate)
        //{
        //    try
        //    {
        //        var user = await _context.Users.AsNoTracking().Include(r => r.Role).FirstOrDefaultAsync(predicate);
        //        if (user == null)
        //            throw new AppException("User not found");

        //        return user;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogException.LogError(ex);
        //        throw;
        //    }
        //}

        public async Task<User?> GetByIdAsync(Guid id)
        {
            try
            {
                var user = await _context.Users.AsNoTracking().Include(r => r.Role).FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                    throw new AppException("User not found");

                return user;
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw;
            }
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            // cần trả ra đủ role và permission để generate token cho user
            try
            {
                var user = await _context.Users
                .Include(u => u.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.UserName == username);

                if (user == null)
                    throw new AppException("User not found");

                return user;
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw;
            }
            
        }

        public IQueryable<User> Query() => _context.Users.AsQueryable().Include(u => u.Role);

        public async Task<User> UpdateAsync(Guid id, User entity)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                    throw new AppException("User not found");

                _context.Entry(existingUser).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
                return existingUser;
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw;
            }

        }
    }
}

// các hàm get có thêm .ThenInclude(ur => ur.Role) là do User và Role có mối quan hệ n-n
// khi đó xuất iện bảng UserRole nếu chỉ Include thôi thì chỉ truy cập đến UserRole nếu muốn
// lấy ra các role của từng user thì thêm .ThenInclude(ur => ur.Role) cái nữa
