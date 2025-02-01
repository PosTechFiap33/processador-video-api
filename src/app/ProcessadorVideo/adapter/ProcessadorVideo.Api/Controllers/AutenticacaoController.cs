using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProcessadorVideo.Application.DTOs;
using ProcessadorVideo.Application.UseCases;
using ProcessadorVideo.Domain.DomainObjects.Exceptions;

namespace ProcessadorVideo.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly ILogger<AutenticacaoController> _logger;

    public AutenticacaoController(ILogger<AutenticacaoController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] AutenticacaoDTO autenticaao,
                                           [FromServices] IAutenticarUseCase useCase)
    {
        try
        {
            var token = await useCase.Executar(autenticaao.Usuario, autenticaao.Senha);
            return Ok(new { token });
        }
        catch (AutenticacaoException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ocorreu um erro ao realizar a autenticação:{ex.Message}");
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
