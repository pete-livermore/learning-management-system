using System.Net.Mime;
using Application.Common.Dtos;
using Application.UseCases.Users.Commands;
using Application.UseCases.Users.Dtos;
using Application.UseCases.Users.Errors;
using Application.UseCases.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.v1.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private ObjectResult GenericErrorResponse() =>
            StatusCode(500, new { message = "An unexpected error occurred." });

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateUserDto createUserDto)
        {
            var result = await _mediator.Send(
                new CreateUserCommand() { CreateCommand = createUserDto }
            );

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    UserErrors.ConflictCode => Conflict(result.Error),
                    _ => GenericErrorResponse(),
                };
            }

            var newUser = result.Value;
            return CreatedAtAction(nameof(Create), new { id = newUser.Id }, newUser);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse>> GetUsers([FromQuery] GetUsersQueryDto query)
        {
            var filters = new UserFiltersDto() { Email = query.Filters.Email };
            var paginationParams = new PaginationParamsDto()
            {
                PageIndex = query.Start ?? 1,
                PageSize = query.Pages ?? 30,
            };

            var findUsersResult = await _mediator.Send(
                new GetUsersQuery() { Filters = filters, Pagination = paginationParams }
            );

            if (findUsersResult.IsFailure)
            {
                return GenericErrorResponse();
            }

            return new ApiResponse(findUsersResult.Value);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetById(int id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery() { UserId = id });
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    UserErrors.NotFoundCode => NotFound(result.Error),
                    _ => GenericErrorResponse(),
                };
            }

            return new ApiResponse(result.Value);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            var result = await _mediator.Send(
                new UpdateUserCommand() { UserId = id, UpdateCommand = updateUserDto }
            );

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    UserErrors.NotFoundCode => NotFound(result.Error),
                    _ => GenericErrorResponse(),
                };
            }
            return NoContent();
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Replace(int id, [FromBody] ReplaceUserDto replaceUserDto)
        {
            var result = await _mediator.Send(
                new ReplaceUserCommand() { UserId = id, ReplaceCommand = replaceUserDto }
            );

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    UserErrors.NotFoundCode => NotFound(),
                    _ => GenericErrorResponse(),
                };
            }
            return CreatedAtAction(nameof(Replace), new { id }, result.Value);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleteUserResult = await _mediator.Send(new DeleteUserCommand() { UserId = id });

            if (!deleteUserResult.IsFailure)
            {
                return deleteUserResult.Error.Code switch
                {
                    UserErrors.NotFoundCode => NotFound(deleteUserResult.Error),
                    _ => GenericErrorResponse(),
                };
            }

            return NoContent();
        }
    }
}
