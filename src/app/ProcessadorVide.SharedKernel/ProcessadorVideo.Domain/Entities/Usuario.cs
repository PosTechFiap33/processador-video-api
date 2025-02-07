using System.Security.Claims;
using ProcessadorVideo.Domain.DomainObjects;

namespace ProcessadorVideo.Domain.Entities;

public enum Perfil{
    Administrador = 1
}


public class Usuario : Entity, IAggregateRoot
{
    public string NomeIdentificacao { get; private set; }
    public string Senha { get; private set; }
    public string Email { get; private set; }
    public Perfil Perfil { get; private set; }

    protected Usuario() { }

    public Usuario(string nomeIdentificacao, string senha, string email, Perfil perfil)
    {
        NomeIdentificacao = nomeIdentificacao;
        Senha = senha;
        Email = email;
        Perfil = perfil;

        AssertionConcern.AssertArgumentNotEmpty(nomeIdentificacao, "O nome de identificação não pode estar vazio!");
        AssertionConcern.AssertArgumentNotEmpty(Senha, "A senha não pode estar vazio!");
        AssertionConcern.AssertArgumentNotEmpty(Email, "O email não pode estar vazio!");
        AssertionConcern.AssertArgumentNotNull(Perfil, "O perfil não pode estar vazio!");
    }

    public bool Autenticar(string senha)
    {
        return Senha.Equals(senha);
    }
}
