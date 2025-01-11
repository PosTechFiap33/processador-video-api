using Microsoft.AspNetCore.Mvc;
using ProcessadorVideo.Application.UseCases;

namespace FiAPProcessaVideo.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class VideoController : ControllerBase
{
    [HttpPost]
    [RequestSizeLimit(900_000_000)] // Limite de 900 MB
    public async Task<IActionResult> ConverterVideo(ICollection<IFormFile> videoFile,
                                                   [FromForm] Guid usuarioId,          
                                                   [FromServices] IConverterVideoParaImagemUseCase useCase)
    {
        try
        {
            if (videoFile == null || !videoFile.Any())
                return BadRequest("A valid video file is required.");

            var zipBytes = await useCase.Executar(videoFile, usuarioId);

            return File(zipBytes, "application/zip", $"frame_{usuarioId}.zip");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
