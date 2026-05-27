using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Cotizacion.Infrastructure.Persistence.Context;

public sealed class CotizacionDbContextFactory : IDesignTimeDbContextFactory<CotizacionDbContext>
{
    public CotizacionDbContext CreateDbContext(string[] args)
    {
        // dotnet ef se puede ejecutar desde la raíz de la solución o desde el proyecto
        var basePath = Directory.GetCurrentDirectory();
        var apiPath = ResolveApiPath(basePath);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<CotizacionDbContext>();
        optionsBuilder.UseNpgsql(
            configuration.GetConnectionString("PostgreSQL"),
            npgsql => npgsql.MigrationsAssembly(typeof(CotizacionDbContext).Assembly.FullName));

        // Mediator no-op para design time
        return new CotizacionDbContext(optionsBuilder.Options, new NoOpMediator());
    }

    private static string ResolveApiPath(string basePath)
    {
        // Ejecutado desde la raíz de la solución: src/Cotizacion.API
        var fromSolution = Path.Combine(basePath, "src", "Cotizacion.API");
        if (Directory.Exists(fromSolution)) return fromSolution;

        // Ejecutado desde src/Cotizacion.Infrastructure
        var fromProject = Path.Combine(basePath, "..", "Cotizacion.API");
        if (Directory.Exists(fromProject)) return Path.GetFullPath(fromProject);

        throw new InvalidOperationException(
            $"No se encontró el directorio Cotizacion.API desde '{basePath}'. " +
            "Ejecuta 'dotnet ef' desde la raíz de la solución.");
    }
}

file sealed class NoOpMediator : IMediator
{
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        => Task.FromResult(default(TResponse)!);

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest => Task.CompletedTask;

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
        => Task.FromResult<object?>(null);

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
        => AsyncEnumerable.Empty<TResponse>();

    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
        => AsyncEnumerable.Empty<object?>();

    public Task Publish(object notification, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification => Task.CompletedTask;
}
