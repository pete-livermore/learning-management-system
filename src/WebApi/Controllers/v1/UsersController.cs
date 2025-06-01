using Application.Dtos;
using Application.Dtos.User;
using Application.Errors;
using Application.UseCases.Users.Commands;
using Application.UseCases.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.v1.Controllers
{
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

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateUserDto createUserDto)
        {
            var result = await _mediator.Send(
                new CreateUserCommand() { CreateCommand = createUserDto }
            );

            if (result.IsFailure)
            {
                return Enum.Parse<UserErrors.Code>(result.Error.Code) switch
                {
                    UserErrors.Code.Conflict => Conflict(result.Error),
                    _ => GenericErrorResponse(),
                };
            }

            var newUser = result.Value;
            return CreatedAtAction(nameof(Create), new { id = newUser.Id }, newUser);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetById(int id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery() { UserId = id });
            if (result.IsFailure)
            {
                return Enum.Parse<UserErrors.Code>(result.Error.Code) switch
                {
                    UserErrors.Code.NotFound => NotFound(result.Error),
                    _ => GenericErrorResponse(),
                };
            }

            return new ApiResponse(result.Value);
        }
    }
}
