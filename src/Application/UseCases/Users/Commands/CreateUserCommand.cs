using Application.Common.Errors.Factories;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Security;
using Application.UseCases.Security.Dtos;
using Application.UseCases.Users.Dtos;
using Application.Wrappers.Results;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Users.Commands;

public record class CreateUserCommand : IRequest<Result<UserDto>>
{
    public required CreateUserDto CreateCommand { get; init; }
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
        var createUserDto = request.CreateCommand;
        var userEmail = createUserDto.Email;

        var createApplicationUserDto = new CreateApplicationUserDto()
        {
            Email = createUserDto.Email,
            Password = createUserDto.Password,
            Roles = [createUserDto.Role],
        };
        var createApplicationUserResult = await _identityService.CreateUserAsync(
            createApplicationUserDto
        );

        if (createApplicationUserResult.IsFailure)
        {
            return Result<UserDto>.Failure([.. createApplicationUserResult.Errors]);
        }

        var existingDomainUser = await _usersRepository.FindByEmailAsync(userEmail);

        if (existingDomainUser is not null)
        {
            return Result<UserDto>.Failure(
                ResourceError.Conflict($"The user with {userEmail} already exists")
            );
        }

        var applicationUserDto = createApplicationUserResult.Value;
        var domainUser = new User()
        {
            Email = userEmail,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            Role = Enum.Parse<UserRole>(createUserDto.Role),
            ApplicationUserId = applicationUserDto.Id, // Crucial link
        };

        _usersRepository.Add(domainUser);
        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
        {
            return Result<UserDto>.Failure([.. saveResult.Errors]);
        }

        var newUserDto = new UserDto()
        {
            Email = domainUser.Email,
            FirstName = domainUser.FirstName,
            LastName = domainUser.LastName,
            Role = domainUser.Role.ToString(),
            Id = domainUser.Id,
        };

        return Result<UserDto>.Success(newUserDto);
    }
}
