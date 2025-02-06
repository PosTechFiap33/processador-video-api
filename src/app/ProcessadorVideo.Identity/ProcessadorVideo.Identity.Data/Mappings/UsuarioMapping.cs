using Microsoft.EntityFrameworkCore;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Identity.Mappings;

public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.NomeIdentificacao)
               .HasMaxLength(10)
               .IsRequired();

        builder.Property(x => x.Email)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(x => x.Perfil)
               .HasMaxLength(1)
               .IsRequired();

        builder.Property(x => x.Senha)
               .HasMaxLength(100)
               .IsRequired();

        builder.ToTable("Usuario");
    }
}
