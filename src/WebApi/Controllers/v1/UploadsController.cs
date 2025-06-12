using Application.UseCases.Uploads.Commands;
using Application.UseCases.Uploads.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UploadsController : ApiController
    {
        private readonly IMediator _mediator;

        public UploadsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(
            IFormFile file,
            [FromForm] FileMetadataDto fileMetadata
        )
        {
            var uploadResult = await _mediator.Send(
                new CreateFileCommand() { FileContent = file, FileMetadata = fileMetadata }
            );

            if (uploadResult.IsFailure)
            {
                return Problem(uploadResult.Errors);
            }

            return CreatedAtAction(
                nameof(UploadFile),
                new { id = uploadResult.Value.Id },
                uploadResult.Value
            );
        }
    }
}
