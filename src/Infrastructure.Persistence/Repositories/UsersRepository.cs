using Application.Common.Dtos;
using Application.Common.Interfaces.Repositories;
using Application.Users.Dtos;
using Domain.Users.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly LearningManagementSystemDbContext _context;
        private readonly ILogger<UsersRepository> _logger;

        public UsersRepository(
            LearningManagementSystemDbContext context,
            ILogger<UsersRepository> logger
        )
        {
            _context = context;
            _logger = logger;
        }

        public void Add(User newUser)
        {
            _context.DomainUsers.Add(newUser);
        }

        public async Task<User?> FindByIdAsync(
            int id,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                return await _context.DomainUsers.FindAsync(id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with Id {UserId}", id);
                throw;
            }
        }

        public async Task<User?> FindByEmailAsync(
            string email,
            CancellationToken cancellationToken = default
        )
        {
            return await _context.DomainUsers.FirstOrDefaultAsync(
                u => u.Email == email,
                cancellationToken
            );
        }

        public async Task<(List<User>, int totalPages)> FindManyAsync(
            UserFiltersDto? filters = null,
            PaginationParamsDto? pagination = null,
            CancellationToken cancellationToken = default
        )
        {
            var query = _context.DomainUsers.AsQueryable();

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

            var totalItems = await query.CountAsync(cancellationToken);
            int pageSize = pagination?.PageSize ?? 30;
            int pageIndex = pagination?.PageIndex ?? 1;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var users = await query
                .OrderBy(b => b.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (users, totalPages);
        }

        public void Update(User userToUpdate)
        {
            // Alternatively, the DbContext could have an interceptor or overridden SaveChanges method
            // to automatically set audit fields like CreatedAt/UpdatedAt.
            userToUpdate.UpdatedAt = DateTime.UtcNow;
            _context.DomainUsers.Update(userToUpdate);
        }

        public void Delete(User userToDelete)
        {
            _context.DomainUsers.Remove(userToDelete);
        }
    }
}
