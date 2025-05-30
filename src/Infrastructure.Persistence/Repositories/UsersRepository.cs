using Application.Dtos;
using Application.Dtos.User;
using Application.Interfaces.Users;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UsersRepository(LearningManagementSystemDbContext context) : IUsersRepository
    {
        private readonly LearningManagementSystemDbContext _context = context;

        public async Task<User> Add(User newUser)
        {
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        public async Task<User?> FindById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> FindByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<(List<User>, int totalPages)> FindMany(
            UserFiltersDto? filters = null,
            PaginationParamsDto? pagination = null
        )
        {
            var query = _context.Users.AsQueryable();

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.FirstName))
                {
                    query = query.Where(u => u.FirstName == filters.FirstName);
                }

                if (!string.IsNullOrEmpty(filters.LastName))
                {
                    query = query.Where(u => u.LastName == filters.LastName);
                }

                if (!string.IsNullOrEmpty(filters.Email))
                {
                    query = query.Where(u => u.Email == filters.Email);
                }
            }

            var totalItems = await query.CountAsync();
            int pageSize = pagination?.PageSize ?? 30;
            int pageIndex = pagination?.PageIndex ?? 1;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var users = await query
                .OrderBy(b => b.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalPages);
        }

        public async Task<User> Update(User userToUpdate)
        {
            userToUpdate.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(userToUpdate);
            await _context.SaveChangesAsync();

            return userToUpdate;
        }

        public async Task Delete(User userToDelete)
        {
            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();
        }
    }
}
