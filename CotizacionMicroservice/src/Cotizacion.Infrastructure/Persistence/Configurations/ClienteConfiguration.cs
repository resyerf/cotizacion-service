using Cotizacion.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cotizacion.Infrastructure.Persistence.Configurations;

public sealed class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("clientes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Nombre).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Ruc).HasMaxLength(20);
        builder.Property(x => x.Email).HasMaxLength(255);
        builder.Property(x => x.Telefono).HasMaxLength(50);
        builder.Property(x => x.Direccion).HasColumnType("text");
        builder.Property(x => x.Activo).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => x.Ruc).IsUnique().HasFilter("\"Ruc\" IS NOT NULL");
    }
}
