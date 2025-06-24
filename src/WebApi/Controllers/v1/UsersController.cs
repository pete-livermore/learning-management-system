using System.Net.Mime;
using Application.Common.Dtos;
using Application.Users.Commands;
using Application.Users.Commands.Create;
using Application.Users.Commands.Delete;
using Application.Users.Dtos;
using Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using WebApi.Contracts.Users;

namespace WebApi.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<ActionResult> Create(
            [FromBody] CreateUserRequest request,
            CancellationToken cancellationToken
        )
        {
            var createResult = await _mediator.Send(
                new CreateUserCommand()
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Password = request.Password,
                    Role = request.Role,
                },
                cancellationToken
            );

            _logger.LogWarning("Create result: @{createResult}", createResult);

            if (createResult.IsFailure)
            {
                return Problem(createResult.Errors);
            }

            var newUser = createResult.Value;
            return CreatedAtAction(nameof(Create), new { id = newUser.Id }, newUser);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse>> GetUsers(
            [FromQuery] GetUsersRequest request,
            CancellationToken cancellationToken
        )
        {
            var filters = new UserFiltersDto() { Email = request.Filters?.Email };
            var paginationParams = new PaginationParamsDto()
            {
                PageIndex = request.Start ?? 1,
                PageSize = request.Pages ?? 30,
            };

            var getUsersResult = await _mediator.Send(
                new GetUsersQuery() { Filters = filters, Pagination = paginationParams },
                cancellationToken
            );

            if (getUsersResult.IsFailure)
            {
                return Problem(getUsersResult.Errors);
            }

            return new ApiResponse(getUsersResult.Value);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetById(
            int id,
            CancellationToken cancellationToken
        )
        {
            var getByIdResult = await _mediator.Send(
                new GetUserByIdQuery() { UserId = id },
                cancellationToken
            );
            if (getByIdResult.IsFailure)
            {
                return Problem(getByIdResult.Errors);
            }

            return new ApiResponse(getByIdResult.Value);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateUserRequest request,
            CancellationToken cancellationToken
        )
        {
            var updateResult = await _mediator.Send(
                new UpdateUserCommand()
                {
                    UserId = id,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Password = request.Password,
                },
                cancellationToken
            );

            if (updateResult.IsFailure)
            {
                return Problem(updateResult.Errors);
            }
            return NoContent();
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Replace(
            int id,
            [FromBody] ReplaceUserRequest request,
            CancellationToken cancellationToken
        )
        {
            var replaceResult = await _mediator.Send(
                new ReplaceUserCommand()
                {
                    UserId = id,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Password = request.Password,
                    Role = request.Role,
                },
                cancellationToken
            );

            if (replaceResult.IsFailure)
            {
                return Problem(replaceResult.Errors);
            }
            return CreatedAtAction(nameof(Replace), new { id }, replaceResult.Value);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
        {
            var deleteResult = await _mediator.Send(
                new DeleteUserCommand() { UserId = id },
                cancellationToken
            );

            if (!deleteResult.IsFailure)
            {
                return Problem(deleteResult.Errors);
            }

            return NoContent();
        }
    }
}
