using System.IdentityModel.Tokens.Jwt;
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
    private readonly ILogger<ProcessamentoVideoController> _logger;

    public ProcessamentoVideoController(ILogger<ProcessamentoVideoController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [RequestSizeLimit(900_000_000)] // Limite de 900 MB
    public async Task<IActionResult> ConverterVideo(ICollection<IFormFile> videoFile,
                                                   [FromServices] IConverterVideoParaImagemUseCase useCase)
    {
        try
        {
            if (videoFile == null || !videoFile.Any())
                return BadRequest("A valid video file is required.");

            var usuarioId = RecuperarIdUsuario();

            var processamento = await useCase.Executar(videoFile, usuarioId);

            return StatusCode((int)HttpStatusCode.Created, processamento.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao processar o video: {ex.Message}");
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListarProcessamentos([FromServices] IListarProcessamentoUseCase useCase)
    {
        try
        {
            var usuarioId = RecuperarIdUsuario();

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
        catch (ProcessamentoNaoEncontradoException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArquivoNaoEncontradoException ex)
        {
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    private Guid RecuperarIdUsuario()
    {
        var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();
        var token = authorizationHeader.Substring("Bearer ".Length).Trim();
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        var usuarioIdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "Id");
        return Guid.Parse(usuarioIdClaim.Value);
    }
}
