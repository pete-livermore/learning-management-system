using Application.Common.Interfaces.Repositories;
using Domain.Lessons.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Repositories;

public class LessonsRepository : ILessonsRepository
{
    private readonly ILogger<LessonsRepository> _logger;
    private readonly LearningManagementSystemDbContext _dbContext;

    public LessonsRepository(
        LearningManagementSystemDbContext dbContext,
        ILogger<LessonsRepository> logger
    )
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public void Add(Lesson lesson)
    {
        _dbContext.Add(lesson);
    }
}
