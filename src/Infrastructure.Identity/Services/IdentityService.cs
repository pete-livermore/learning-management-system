using System.Text.Json;
using Application.Common.Errors;
using Application.Common.Errors.Factories;
using Application.Common.Interfaces.Token;
using Application.Common.Wrappers.Results;
using Application.Security.Dtos;
using Application.Security.Interfaces;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ITokenService _tokenService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ITokenService tokenService
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }

    private Error GetUserError(IEnumerable<IdentityError> errors, string userEmail)
    {
        foreach (var err in errors)
        {
            string code = err.Code;

            if (code == "DuplicateEmail")
            {
                return ResourceError.Conflict($"User with email {userEmail} already exists");
            }

            if (
                code == "PasswordTooShort"
                || code == "PasswordRequiresNonAlphanumeric"
                || code == "PasswordRequiresDigit"
                || code == "PasswordRequiresLower"
                || code == "PasswordRequiresUpper"
                || code == "PasswordRequiresUniqueChars"
            )
            {
                return ValidationError.InvalidInput("Password does not meet the requirements");
            }

            if (code == "DuplicateRoleName" || code == "InvalidRoleName")
            {
                return ValidationError.InvalidInput("Invalid role name");
            }
        }
        return UnexpectedError.Unknown(errors.First().Code);
        ;
    }

    private Error GetRoleError(IEnumerable<IdentityError> errors, string roleName)
    {
        foreach (var err in errors)
        {
            string code = err.Code;

            if (code == "DuplicateRoleName")
            {
                return ResourceError.Conflict($"The {roleName} role already exists");
            }
            if (code == "InvalidRoleName")
            {
                return ValidationError.InvalidInput($"{roleName} is an invalid role name");
                ;
            }
        }
        return UnexpectedError.Unknown(errors.First().Code);
        ;
    }

    public async Task<Result<ApplicationUserDto>> CreateUserAsync(
        CreateApplicationUserDto createUserDto
    )
    {
        string userEmail = createUserDto.Email;
        var applicationUser = new ApplicationUser()
        {
            Email = userEmail,
            EmailConfirmed = false,
            UserName = userEmail,
        };

        var createResult = await _userManager.CreateAsync(applicationUser, createUserDto.Password);

        if (!createResult.Succeeded)
        {
            return Result<ApplicationUserDto>.Failure(GetUserError(createResult.Errors, userEmail));
        }
        ;

        var existingRoles = await _roleManager.Roles.Select(r => r.Name).ToHashSetAsync();
        var requestedRoles = createUserDto.Roles.Distinct().ToList();
        var invalidRoles = requestedRoles.Where((r) => !existingRoles.Contains(r));

        if (invalidRoles.Any())
        {
            return Result<ApplicationUserDto>.Failure(
                ValidationError.InvalidInput("At least some of the supplied roles were invalid")
            );
        }

        var addRolesResult = await _userManager.AddToRolesAsync(applicationUser, requestedRoles);

        if (!addRolesResult.Succeeded)
        {
            await _userManager.DeleteAsync(applicationUser); // Rollback user if role assignment fails
            return Result<ApplicationUserDto>.Failure(
                UnexpectedError.Unknown(
                    "There was an error adding the given roles to the application user",
                    addRolesResult.Errors.First().Description
                )
            );
        }

        var userRoles = await _userManager.GetRolesAsync(applicationUser);

        var applicationUserDto = new ApplicationUserDto()
        {
            Id = applicationUser.Id,
            Email = applicationUser.Email,
            Roles = userRoles.ToList(),
        };

        return Result<ApplicationUserDto>.Success(applicationUserDto);
    }

    public async Task<Result<ApplicationUserDto>> FindUserByEmailAsync(string userEmail)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user is null)
        {
            return Result<ApplicationUserDto>.Failure(
                ResourceError.NotFound($"Application user with email {userEmail} not found")
            );
        }
        var userRoles = await _userManager.GetRolesAsync(user);

        var applicationUserDto = new ApplicationUserDto()
        {
            Id = user.Id,
            Email = userEmail,
            Roles = userRoles.ToList(),
        };
        return Result<ApplicationUserDto>.Success(applicationUserDto);
    }

    public async Task<Result<ApplicationUserDto>> FindUserByIdAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result<ApplicationUserDto>.Failure(
                ResourceError.NotFound($"Application user with ID {userId} not found")
            );
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var applicationUserDto = new ApplicationUserDto()
        {
            Id = user.Id,
            Email = user.Email!,
            Roles = userRoles.ToList(),
        };
        return Result<ApplicationUserDto>.Success(applicationUserDto);
    }

    public async Task<Result> UpdateUserAsync(
        Guid userId,
        UpdateApplicationUserDto updateApplicationUserDto
    )
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(ResourceError.NotFound($"User with Id {userId} not found"));
        }
        var newEmail = updateApplicationUserDto.Email;

        if (newEmail is not null)
        {
            user.Email = newEmail;
            user.NormalizedEmail = _userManager.NormalizeEmail(newEmail);
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return Result.Failure();
        }

        return Result.Success();
    }

    public async Task<Result<List<string>>> GetUserRolesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Result<List<string>>.Failure(
                ResourceError.NotFound($"Application user with Id {userId} not found")
            );
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Result<List<string>>.Success(roles.ToList());
    }

    public async Task<Result> DeleteUserAsync(Guid userId)
    {
        var userToDelete = await _userManager.FindByIdAsync(userId.ToString());
        if (userToDelete is null)
        {
            return Result.Failure(
                ResourceError.NotFound($"Application user with Id {userId} not found")
            );
        }
        ;
        var deleteResult = await _userManager.DeleteAsync(userToDelete);

        if (!deleteResult.Succeeded)
        {
            return Result.Failure();
        }

        return Result.Success();
    }

    public async Task<Result<ApplicationRoleDto>> CreateRoleAsync(
        CreateApplicationRoleDto createApplicationRoleDto
    )
    {
        string roleName = createApplicationRoleDto.Name;
        var newRole = new ApplicationRole() { Name = roleName };
        var createRoleResult = await _roleManager.CreateAsync(newRole);

        if (!createRoleResult.Succeeded)
        {
            return Result<ApplicationRoleDto>.Failure(
                GetRoleError(createRoleResult.Errors, roleName)
            );
        }
        var roleDto = new ApplicationRoleDto() { Name = roleName };
        return Result<ApplicationRoleDto>.Success(roleDto);
    }

    public async Task<Result<string>> AuthenticateUserAsync(string userEmail, string password)
    {
        var applicationUser = await _userManager.FindByEmailAsync(userEmail);

        if (applicationUser is null || applicationUser.Email is null)
        {
            return Result<string>.Failure(SecurityError.Unauthorized("User not found"));
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(applicationUser, password);
        if (!isValidPassword)
        {
            return Result<string>.Failure(
                SecurityError.InvalidCredentials(
                    "The supplied credentials are invalid",
                    "Invalid password"
                )
            );
        }

        var userRoles = await _userManager.GetRolesAsync(applicationUser);

        string token = _tokenService.Generate(
            new TokenDataDto()
            {
                UserId = applicationUser.Id,
                Email = applicationUser.Email,
                Roles = userRoles.ToList(),
            }
        );

        return Result<string>.Success(token);
    }

    public async Task<Result<ApplicationRoleDto>> FindRoleByNameAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);

        if (role is null)
        {
            return Result<ApplicationRoleDto>.Failure(
                ResourceError.NotFound($"Role {roleName} does not exist")
            );
        }

        var applicationRoleDto = new ApplicationRoleDto() { Name = role.Name! };

        return Result<ApplicationRoleDto>.Success(applicationRoleDto);
    }
}
