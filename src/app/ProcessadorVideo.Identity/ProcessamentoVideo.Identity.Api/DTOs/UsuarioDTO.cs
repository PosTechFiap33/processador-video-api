using System;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Identity.Api.DTOs;

public class UsuarioDTO
{
    public string Usuario { get; set; }
    public string Senha { get; set; }
    public string Email { get; set; }
    public Perfil Perfil { get; set; }
}
