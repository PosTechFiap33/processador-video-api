using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProcessadorVideo.Domain.Adapters.Services;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Identity.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Gerar(Usuario usuario, string secret)
    {
        var tokenConfig = _configuration.GetSection("Token");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new List<Claim>
        {
            new Claim("nomeIdentificacao", usuario.NomeIdentificacao),
            new Claim("perfil", ((int)usuario.Perfil).ToString()),
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString().ToLower())
        }),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(tokenConfig["Minutes"])),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

}
