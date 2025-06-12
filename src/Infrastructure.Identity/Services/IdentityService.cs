using System.Text.Json;
using Application.Common.Errors;
using Application.Common.Interfaces.Security;
using Application.Common.Interfaces.Token;
using Application.UseCases.Security.Dtos;
using Application.UseCases.Security.Errors;
using Application.UseCases.Users.Dtos;
using Application.Wrappers.Results;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Infrastructure.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    private Error GetIdentityError(IEnumerable<IdentityError> errors, string userEmail)
    {
        foreach (var err in errors)
        {
            string code = err.Code;

            if (code == "DuplicateEmail")
            {
                return IdentityErrors.Conflict(userEmail);
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
                return IdentityErrors.Validation(
                    userEmail,
                    "Password does not meet the requirements"
                );
            }
        }
        return IdentityErrors.Unexpected();
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
            Console.WriteLine(
                $"IDENTITY ERRORs => {JsonSerializer.Serialize(createResult.Errors)}"
            );
            return Result<ApplicationUserDto>.Failure(
                GetIdentityError(createResult.Errors, userEmail)
            );
        }
        ;

        var existingRoles = await _roleManager.Roles.Select(r => r.Name).ToHashSetAsync();
        var requestedRoles = createUserDto.Roles.Distinct().ToList();
        var invalidRoles = requestedRoles.Where((r) => !existingRoles.Contains(r));

        if (invalidRoles.Any())
        {
            return Result<ApplicationUserDto>.Failure(IdentityErrors.Validation(userEmail));
        }

        var addRolesResult = await _userManager.AddToRolesAsync(applicationUser, requestedRoles);

        if (!addRolesResult.Succeeded)
        {
            await _userManager.DeleteAsync(applicationUser); // Rollback user if role assignment fails
            return Result<ApplicationUserDto>.Failure(new Error(ErrorType.Unexpected, "", ""));
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

    public async Task<Result> UpdateUser(
        Guid userId,
        UpdateApplicationUserDto updateApplicationUserDto
    )
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(IdentityErrors.NotFound(userId));
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
            return Result<List<string>>.Failure(IdentityErrors.NotFound(userId));
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Result<List<string>>.Success(roles.ToList());
    }

    public async Task<Result> DeleteAsync(Guid userId)
    {
        var userToDelete = await _userManager.FindByIdAsync(userId.ToString());
        if (userToDelete is null)
        {
            return Result.Failure(IdentityErrors.NotFound(userId));
        }
        ;
        var deleteResult = await _userManager.DeleteAsync(userToDelete);

        if (!deleteResult.Succeeded)
        {
            return Result.Failure();
        }

        return Result.Success();
    }

    public async Task<Result<string>> AuthenticateUserAsync(string userEmail, string password)
    {
        var applicationUser = await _userManager.FindByEmailAsync(userEmail);

        if (applicationUser is null || applicationUser.Email is null)
        {
            return Result<string>.Failure(SecurityErrors.Unauthorized());
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            applicationUser,
            password,
            false
        );

        if (signInResult.IsLockedOut)
        {
            return Result<string>.Failure(
                IdentityErrors.Forbidden(
                    "This account has been locked out, please try again later."
                )
            );
        }

        if (signInResult.IsNotAllowed)
        {
            return Result<string>.Failure(
                IdentityErrors.Forbidden("This user is not able to sign in")
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
}
