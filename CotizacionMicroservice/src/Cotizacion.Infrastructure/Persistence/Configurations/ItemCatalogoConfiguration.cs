using Cotizacion.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cotizacion.Infrastructure.Persistence.Configurations;

public sealed class ItemCatalogoConfiguration : IEntityTypeConfiguration<ItemCatalogo>
{
    public void Configure(EntityTypeBuilder<ItemCatalogo> builder)
    {
        builder.ToTable("items_catalogo");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ActividadId).IsRequired();
        builder.Property(x => x.Codigo).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Descripcion).IsRequired().HasColumnType("text");
        builder.Property(x => x.Unidad).IsRequired().HasMaxLength(30);
        builder.Property(x => x.PrecioBase).IsRequired().HasColumnType("numeric(14,2)");
        builder.Property(x => x.Activo).IsRequired().HasDefaultValue(true);

        builder.HasIndex(x => x.Codigo).IsUnique();
        builder.HasIndex(x => x.ActividadId);

        builder.HasOne(x => x.Actividad)
            .WithMany()
            .HasForeignKey(x => x.ActividadId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
