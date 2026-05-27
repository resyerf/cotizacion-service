using Cotizacion.Application.Extensions;
using Cotizacion.Infrastructure.Extensions;
using Cotizacion.Infrastructure.Persistence.Context;
using Cotizacion.API.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(
                  "http://localhost:4200",
                  "https://localhost:4200",
                  "http://localhost:4202",
                  "https://cotizacion-coingec.resyerf.com")
              .AllowAnyHeader()
              .AllowAnyMethod()));

builder.Services.AddControllers();

builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Cotizacion API",
            Version = "v1",
            Description = """
                API REST para la gestión integral de cotizaciones comerciales.

                Permite administrar:
                - **Clientes**: registro y consulta
                - **Catálogo**: actividades e ítems con precios base
                - **Cotizaciones**: creación, gestión de partidas, cambio de estado y exportación (PDF/Excel)
                """,
            Contact = new OpenApiContact
            {
                Name = "Equipo Coingec",
                Email = "freyseripanaque@gmail.com"
            }
        };
        return Task.CompletedTask;
    });
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CotizacionDbContext>();
    await db.Database.MigrateAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapOpenApi();

app.MapScalarApiReference("/docs", options =>
{
    options.Title = "Cotizacion API";
    options.Theme = ScalarTheme.Purple;
    options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    options.WithOpenApiRoutePattern("/openapi/v1.json");
});

app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
