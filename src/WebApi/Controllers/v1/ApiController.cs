using Application.Common.Errors;
using Application.Common.Errors.Factories;
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
            int statusCode = StatusCodes.Status500InternalServerError;
            var errorCode = error.Code;
            var errorType = error.Type;

            if (errorType == ErrorType.Security)
            {
                if (errorCode == SecurityError.ForbiddenCode)
                {
                    statusCode = StatusCodes.Status403Forbidden;
                }
                else
                {
                    statusCode = StatusCodes.Status401Unauthorized;
                }
            }

            if (errorType == ErrorType.ResourcePersistence)
            {
                if (errorCode == ResourceError.ConflictCode)
                {
                    statusCode = StatusCodes.Status409Conflict;
                }
                if (errorCode == ResourceError.NotFoundCode)
                {
                    statusCode = StatusCodes.Status404NotFound;
                }
            }

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
