using IdentityService.Application.Interfaces.IRepositories;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedLibrarySolution.Exceptions;
using SharedLibrarySolution.Logs;
using System.Linq.Expressions;

namespace IdentityService.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IdentityDbContext _context;
        public RoleRepository(IdentityDbContext context) => _context = context;

        public async Task<Role> CreateAsync(Role entity)
        {
            try {
                await _context.Roles.AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex) {
                LogException.LogError(ex);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var existingRole = await _context.Roles.FindAsync(id);
                if (existingRole == null)
                    throw new AppException("Role not found");

                _context.Roles.Remove(existingRole);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw;
            }
        }

        public async Task<IEnumerable<Role>> GetAllAsync(Expression<Func<Role, bool>>? predicate)
        {
            try
            {
                var listRoles = _context.Roles.ToListAsync();
                return await listRoles.ContinueWith(t => (IEnumerable<Role>)t.Result);
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw;
            }
        }

        //public async Task<Role?> GetByAsync(Expression<Func<Role, bool>> predicate)
        //{
        //    try
        //    {
        //        var user = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(predicate);
        //        if (user == null)
        //            throw new AppException("Role not found");

        //        return user;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogException.LogError(ex);
        //        throw;
        //    }
        //}

        public async Task<Role?> GetByIdAsync(Guid id)
        {
            try
            {
                var user = await _context.Roles.FirstOrDefaultAsync(r=>r.Id==id);
                if (user == null)
                    throw new AppException("Role not found");

                return user;
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw;
            }
        }

        public IQueryable<Role> Query()
        {
            throw new NotImplementedException();
        }

        public async Task<Role> UpdateAsync(Guid id, Role entity)
        {
            try
            {
                var existingRole = await _context.Roles.FindAsync(id);
                if (existingRole == null)
                    throw new AppException("Role not found");

                _context.Entry(existingRole).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
                return existingRole;
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw;
            }
        }
    }
}
