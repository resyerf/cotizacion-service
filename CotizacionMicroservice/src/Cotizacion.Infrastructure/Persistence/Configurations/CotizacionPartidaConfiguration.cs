using Cotizacion.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cotizacion.Infrastructure.Persistence.Configurations;

public sealed class CotizacionPartidaConfiguration : IEntityTypeConfiguration<CotizacionPartida>
{
    public void Configure(EntityTypeBuilder<CotizacionPartida> builder)
    {
        builder.ToTable("cotizacion_partidas");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.CotizacionId).IsRequired();
        builder.Property(x => x.ItemCatalogoId).IsRequired();
        builder.Property(x => x.PrecioUnitario).IsRequired().HasColumnType("numeric(14,2)");
        builder.Property(x => x.Cantidad).HasColumnType("numeric(10,3)");

        builder.Ignore(x => x.Subtotal);

        builder.HasIndex(x => new { x.CotizacionId, x.ItemCatalogoId }).IsUnique();
        builder.HasIndex(x => x.ItemCatalogoId);

        builder.HasOne(x => x.ItemCatalogo)
            .WithMany()
            .HasForeignKey(x => x.ItemCatalogoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
