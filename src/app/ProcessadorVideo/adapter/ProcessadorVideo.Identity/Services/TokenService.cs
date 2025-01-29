using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProcessadorVideo.Domain.Adapters.Services;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Identity.Services;

public class TokenService : ITokenService
{
    public string Gerar(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("SecretKey") ?? "default_secret_value");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim("nomeIdentificacao", usuario.NomeIdentificacao),
                new Claim("perfil", ((int)usuario.Perfil).ToString()),
                new Claim(ClaimTypes.Role, usuario.Perfil.ToString().ToLower()),
            ]),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
