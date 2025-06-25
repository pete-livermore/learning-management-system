namespace Application.Common.Interfaces.Repositories;

using System.Threading;
using System.Threading.Tasks;
using Application.Common.Wrappers.Results;

public interface IUnitOfWork
{
    /// <summary>
    /// Asynchronously saves all changes made in this unit of work to the underlying data store.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the number of state entries written to the database,
    /// or a failure result if an error occurred during saving.
    /// </returns>
    Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default);
}
