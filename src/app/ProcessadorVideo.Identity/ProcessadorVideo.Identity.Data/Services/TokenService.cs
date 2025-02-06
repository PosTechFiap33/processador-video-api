using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ProcessadorVideo.Domain.Adapters.Services;
using ProcessadorVideo.Domain.DomainObjects.Exceptions;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Identity.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IConfiguration configuration,
                        ILogger<TokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string Gerar(Usuario usuario, string secret)
    {
        try
        {
            var tokenConfig = _configuration.GetSection("Token");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
            {
                new Claim("Id", usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Perfil.ToString().ToLower())
            }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(tokenConfig["Minutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao gerar o token: {ex.Message}");
            throw new IntegrationException("Ocorreu um erro ao gerar o token!");
        }
    }
}
