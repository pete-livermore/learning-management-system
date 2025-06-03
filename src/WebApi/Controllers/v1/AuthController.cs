using Application.UseCases.Security.Commands;
using Application.UseCases.Security.Dtos;
using Application.UseCases.Security.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResult = await _mediator.Send(
                new LoginCommand() { Email = loginDto.Email, Password = loginDto.Password }
            );

            if (loginResult.IsFailure)
            {
                if (
                    loginResult.Error.Code == SecurityErrors.InvalidUserCode
                    || loginResult.Error.Code == SecurityErrors.InvalidPasswordCode
                )
                {
                    return Unauthorized();
                }
                return StatusCode(500, "There was an unexpected authentication error");
            }

            return Ok(new { Token = loginResult.Value });
        }
    }
}
