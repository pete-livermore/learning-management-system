using Application.Security.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class AuthController : ApiController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var loginResult = await _mediator.Send(
                new LoginCommand() { Email = request.Email, Password = request.Password }
            );

            if (loginResult.IsFailure)
            {
                return Problem(loginResult.Errors);
            }

            return Ok(new { Token = loginResult.Value });
        }
    }
}
