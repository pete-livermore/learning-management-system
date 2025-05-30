using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Contexts
{
    public class LearningManagementSystemDbContext : DbContext
    {
        public LearningManagementSystemDbContext(
            DbContextOptions<LearningManagementSystemDbContext> options
        )
            : base(options) { }
    }
}
