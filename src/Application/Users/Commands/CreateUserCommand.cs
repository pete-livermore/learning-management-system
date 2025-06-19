using Application.Common.Errors.Factories;
using Application.Common.Interfaces.Repositories;
using Application.Common.Wrappers.Results;
using Application.Security.Dtos;
using Application.Security.Interfaces;
using Application.Users.Dtos;
using Domain.Enums;
using Domain.Users.Entities;
using Domain.Users.Enums;
using Domain.Users.Events;
using MediatR;

namespace Application.Users.Commands;

public record class CreateUserCommand : IRequest<Result<UserDto>>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Role { get; set; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IIdentityService identityService,
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork
    )
    {
        _identityService = identityService;
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var userEmail = request.Email;
        var existingDomainUser = await _usersRepository.FindByEmailAsync(userEmail);

        if (existingDomainUser is not null)
        {
            return Result<UserDto>.Failure(
                ResourceError.Conflict($"The user with {userEmail} already exists")
            );
        }

        var createApplicationUserDto = new CreateApplicationUserDto()
        {
            Email = request.Email,
            Password = request.Password,
            Roles = [request.Role],
        };
        var createApplicationUserResult = await _identityService.CreateUserAsync(
            createApplicationUserDto
        );

        if (createApplicationUserResult.IsFailure)
        {
            return Result<UserDto>.Failure([.. createApplicationUserResult.Errors]);
        }

        var applicationUserDto = createApplicationUserResult.Value;

        var newDomainUser = new User()
        {
            Email = userEmail,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = Enum.Parse<UserRole>(request.Role),
            ApplicationUserId = applicationUserDto.Id,
        };

        _usersRepository.Add(newDomainUser);
        newDomainUser.AddDomainEvent(new UserCreatedEvent(newDomainUser.Id));
        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
        {
            return Result<UserDto>.Failure([.. saveResult.Errors]);
        }

        var newUserDto = new UserDto()
        {
            Email = newDomainUser.Email,
            FirstName = newDomainUser.FirstName,
            LastName = newDomainUser.LastName,
            Role = newDomainUser.Role.ToString(),
            Id = newDomainUser.Id,
        };

        return Result<UserDto>.Success(newUserDto);
    }
}
