namespace Infrastructure.Persistence.Repositories;

using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Errors;
using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Repositories;
using Application.Wrappers.Results;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

public class UnitOfWork : IUnitOfWork
{
    private readonly LearningManagementSystemDbContext _context;

    public UnitOfWork(LearningManagementSystemDbContext context)
    {
        _context = context;
    }

    private Error GetDbUpdateError(DbUpdateException ex)
    {
        var innerMessage = ex.InnerException?.Message ?? ex.Message;

        if (
            innerMessage.Contains("UNIQUE KEY constraint")
            || innerMessage.Contains("duplicate key")
            || innerMessage.Contains("unique constraint failed")
            || innerMessage.Contains("duplicate entry")
        )
        {
            return PersistenceErrors.Conflict();
        }
        else if (
            innerMessage.Contains("FOREIGN KEY constraint")
            || innerMessage.Contains("REFERENCES constraint")
        )
        {
            return PersistenceErrors.InvalidReference();
        }
        else if (
            innerMessage.Contains("truncated")
            || innerMessage.Contains("NULL into column")
            || innerMessage.Contains("value too large")
        )
        {
            return PersistenceErrors.DataIntegrity();
        }
        else
        {
            return PersistenceErrors.Unexpected(ex.Message);
        }
    }

    public async Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(result);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result<int>.Failure(PersistenceErrors.Concurrency(ex.Message));
        }
        catch (DbUpdateException ex)
        {
            return Result<int>.Failure(GetDbUpdateError(ex));
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(PersistenceErrors.Unexpected(ex.Message));
        }
    }
}
