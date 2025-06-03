using System.Net.Mime;
using System.Text.Json;
using Application.Common.Dtos;
using Application.UseCases.Users.Commands;
using Application.UseCases.Users.Dtos;
using Application.UseCases.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.v1;

namespace WebApi.v1.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
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
        public async Task<ActionResult> Create([FromBody] CreateUserDto createUserDto)
        {
            var createResult = await _mediator.Send(
                new CreateUserCommand() { CreateCommand = createUserDto }
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
        public async Task<ActionResult<ApiResponse>> GetUsers([FromQuery] GetUsersQueryDto query)
        {
            var filters = new UserFiltersDto() { Email = query.Filters.Email };
            var paginationParams = new PaginationParamsDto()
            {
                PageIndex = query.Start ?? 1,
                PageSize = query.Pages ?? 30,
            };

            var getUsersResult = await _mediator.Send(
                new GetUsersQuery() { Filters = filters, Pagination = paginationParams }
            );

            if (getUsersResult.IsFailure)
            {
                return Problem(getUsersResult.Errors);
            }

            return new ApiResponse(getUsersResult.Value);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetById(int id)
        {
            var getByIdResult = await _mediator.Send(new GetUserByIdQuery() { UserId = id });
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
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            var updateResult = await _mediator.Send(
                new UpdateUserCommand() { UserId = id, UpdateCommand = updateUserDto }
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
        public async Task<IActionResult> Replace(int id, [FromBody] ReplaceUserDto replaceUserDto)
        {
            var replaceResult = await _mediator.Send(
                new ReplaceUserCommand() { UserId = id, ReplaceCommand = replaceUserDto }
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
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleteResult = await _mediator.Send(new DeleteUserCommand() { UserId = id });

            if (!deleteResult.IsFailure)
            {
                return Problem(deleteResult.Errors);
            }

            return NoContent();
        }
    }
}
