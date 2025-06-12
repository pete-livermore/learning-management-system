using Application.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApi.Controllers.v1
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        protected ActionResult Problem(IReadOnlyList<Error> errors)
        {
            return errors.Any(err => err.Type == ErrorType.Validation)
                ? ValidationProblem(errors)
                : Problem(errors[0]);
        }

        private ObjectResult Problem(Error error)
        {
            var statusCode = error.Type switch
            {
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError,
            };

            return Problem(statusCode: statusCode, title: error.Description);
        }

        private ActionResult ValidationProblem(IReadOnlyList<Error> errors)
        {
            var modelStateDictionary = new ModelStateDictionary();
            foreach (var error in errors)
            {
                modelStateDictionary.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem(modelStateDictionary);
        }
    }
}
