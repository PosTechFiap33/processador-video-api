using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Domain.Adapters.Services;

public interface ITokenService
{
    string Gerar(Usuario usuario, string secret);
}
