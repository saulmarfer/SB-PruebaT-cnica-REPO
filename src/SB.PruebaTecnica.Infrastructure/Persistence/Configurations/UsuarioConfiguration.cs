using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SB.PruebaTecnica.Domain.Entities;

namespace SB.PruebaTecnica.Infrastructure.Persistence.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.NombreUsuario).HasMaxLength(100).IsRequired();
            builder.HasIndex(u => u.NombreUsuario).IsUnique();

            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.Rol).HasConversion<int>().IsRequired();
        }
    }
}
