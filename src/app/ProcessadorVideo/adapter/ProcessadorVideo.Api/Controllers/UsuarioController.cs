using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProcessadorVideo.Application.DTOs;
using ProcessadorVideo.Application.UseCases;
using ProcessadorVideo.Domain.DomainObjects;

namespace ProcessadorVideo.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(ILogger<UsuarioController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UsuarioDTO usuario,
                                           [FromServices] ICadastrarUsuarioUseCase useCase)
    {
        try
        {
            await useCase.Executar(usuario);
            return StatusCode((int)HttpStatusCode.Created, "Usuario cadastrado!");
        }
        catch (DomainException ex)
        {
            _logger.LogError($"Ocorreu um erro ao realizar o cadastro do usuario:{ex.Message}");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ocorreu um erro ao realizar o cadastro do usuario:{ex.Message}");
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
