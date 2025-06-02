using Application.Common.Interfaces.Repositories;
using Application.UseCases.Users.Errors;
using Application.Wrappers.Results;
using MediatR;

namespace Application.UseCases.Users.Commands;

public record class DeleteUserCommand : IRequest<Result>
{
    public required int UserId { get; init; }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IUsersRepository _usersRepository;

    public DeleteUserCommandHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var userToDelete = await _usersRepository.FindById(userId);

        if (userToDelete is null)
        {
            return Result.Failure(UserErrors.NotFound(userId));
        }

        await _usersRepository.Delete(userToDelete);

        return Result.Success();
    }
}
