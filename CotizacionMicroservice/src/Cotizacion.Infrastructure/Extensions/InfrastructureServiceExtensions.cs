using Cotizacion.Application.Common;
using Cotizacion.Application.Services;
using Cotizacion.Domain.Interfaces.Repositories;
using Cotizacion.Infrastructure.Persistence;
using Cotizacion.Infrastructure.Persistence.Context;
using Cotizacion.Infrastructure.Persistence.Repositories;
using Cotizacion.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Cotizacion.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CotizacionDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("PostgreSQL"),
                npgsql => npgsql.MigrationsAssembly(typeof(CotizacionDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IActividadRepository, ActividadRepository>();
        services.AddScoped<IItemCatalogoRepository, ItemCatalogoRepository>();
        services.AddScoped<ICotizacionRepository, CotizacionRepository>();
        services.AddScoped<IPdfService, QuestPdfService>();
        services.AddScoped<IExcelService, ClosedXmlExcelService>();

        return services;
    }
}
