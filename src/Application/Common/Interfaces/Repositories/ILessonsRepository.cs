using Domain.Lessons.Entities;

namespace Application.Common.Interfaces.Repositories;

public interface ILessonsRepository
{
    public void Add(Lesson lesson);
}
