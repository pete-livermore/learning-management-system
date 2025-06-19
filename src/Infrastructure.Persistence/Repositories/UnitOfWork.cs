namespace Infrastructure.Persistence.Repositories;

using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Errors;
using Application.Common.Errors.Factories;
using Application.Common.Interfaces.Repositories;
using Application.Common.Wrappers.Results;
using Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class UnitOfWork : IUnitOfWork
{
    private readonly LearningManagementSystemDbContext _context;
    private readonly IMediator _mediator;

    public UnitOfWork(LearningManagementSystemDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
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
            return ResourceError.Conflict(ex.Message);
        }
        else if (
            innerMessage.Contains("FOREIGN KEY constraint")
            || innerMessage.Contains("REFERENCES constraint")
            || innerMessage.Contains("truncated")
            || innerMessage.Contains("NULL into column")
            || innerMessage.Contains("value too large")
        )
        {
            return ValidationError.InvalidInput(ex.Message);
        }
        else
        {
            return UnexpectedError.Unknown(ex.Message);
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
            return Result<int>.Failure(ResourceError.Concurrency(ex.Message));
        }
        catch (DbUpdateException ex)
        {
            return Result<int>.Failure(GetDbUpdateError(ex));
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(UnexpectedError.Unknown(ex.Message));
        }
    }
}
