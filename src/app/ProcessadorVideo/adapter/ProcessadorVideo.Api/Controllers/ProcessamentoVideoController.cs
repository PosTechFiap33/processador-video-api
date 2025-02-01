using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProcessadorVideo.Application.DTOs;
using ProcessadorVideo.Application.UseCases;
using ProcessadorVideo.Domain.DomainObjects.Exceptions;

namespace FiAPProcessaVideo.Api.Controllers;

[ApiController]
[Authorize(Roles = "administrador")]
[Route("[controller]")]
public class ProcessamentoVideoController : ControllerBase
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

            await useCase.Executar(videoFile, usuarioId);

            return StatusCode((int)HttpStatusCode.Created, "Processamento iniciado!");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListarProcessamentos([FromQuery] Guid usuarioId,
                                                          [FromServices] IListarProcessamentoUseCase useCase)
    {
        try
        {
            if (Guid.Empty == usuarioId)
                return BadRequest("Id do usuário não foi informado!");

            var listaProcessamento = await useCase.Executar(usuarioId);

            return Ok(listaProcessamento);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet("{processamentoId}/download")]
    public async Task<IActionResult> BaixarArquivo(Guid processamentoId,
                                                  [FromServices] IConsultarArquivoZipUseCase useCase)
    {
        try
        {
            if (Guid.Empty == processamentoId)
                return BadRequest("Id do processamento não foi informado!");

            ArquivoZipDTO arquivo = await useCase.Executar(processamentoId);

            return File(arquivo.Conteudo, "application/zip", arquivo.Nome);
        }
        catch(ProcessamentoNaoEncontradoException ex){
            return NotFound(ex.Message);
        }
        catch(ArquivoNaoEncontradoException ex){
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

}
