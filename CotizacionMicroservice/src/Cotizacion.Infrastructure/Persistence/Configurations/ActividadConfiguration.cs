using Cotizacion.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cotizacion.Infrastructure.Persistence.Configurations;

public sealed class ActividadConfiguration : IEntityTypeConfiguration<Actividad>
{
    public void Configure(EntityTypeBuilder<Actividad> builder)
    {
        builder.ToTable("actividades");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Codigo).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Nombre).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Orden).IsRequired();
        builder.Property(x => x.Activo).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => x.Codigo).IsUnique();

        builder.Ignore(x => x.Items);
    }
}
