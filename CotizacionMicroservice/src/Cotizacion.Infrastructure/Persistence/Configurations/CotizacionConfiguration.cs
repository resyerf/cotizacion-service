using Cotizacion.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cotizacion.Infrastructure.Persistence.Configurations;

public sealed class CotizacionConfiguration : IEntityTypeConfiguration<Domain.Entities.Cotizacion>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Cotizacion> builder)
    {
        builder.ToTable("cotizaciones");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Numero).IsRequired().HasMaxLength(30);
        builder.Property(x => x.ClienteId).IsRequired();
        builder.Property(x => x.Proyecto).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Ubicacion).HasMaxLength(255);
        builder.Property(x => x.Fecha).IsRequired();
        builder.Property(x => x.FechaValidez);
        builder.Property(x => x.Estado).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Moneda).IsRequired().HasConversion<string>().HasMaxLength(3);
        builder.Property(x => x.Notas).HasColumnType("text");
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.Ignore(x => x.Total);

        builder.HasIndex(x => x.Numero).IsUnique();
        builder.HasIndex(x => x.ClienteId);
        builder.HasIndex(x => x.Estado);

        builder.HasOne(x => x.Cliente)
            .WithMany()
            .HasForeignKey(x => x.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Partidas)
            .WithOne()
            .HasForeignKey(x => x.CotizacionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
