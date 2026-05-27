# ─────────────────────────────────────────────────────────────────
# Stage 1 — Build
# ─────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project manifests first so this layer is cached until they change
COPY CotizacionMicroservice/CotizacionMicroservice.slnx \
     ./CotizacionMicroservice/

COPY CotizacionMicroservice/src/Cotizacion.Domain/Cotizacion.Domain.csproj \
     ./CotizacionMicroservice/src/Cotizacion.Domain/

COPY CotizacionMicroservice/src/Cotizacion.Application/Cotizacion.Application.csproj \
     ./CotizacionMicroservice/src/Cotizacion.Application/

COPY CotizacionMicroservice/src/Cotizacion.Infrastructure/Cotizacion.Infrastructure.csproj \
     ./CotizacionMicroservice/src/Cotizacion.Infrastructure/

COPY CotizacionMicroservice/src/Cotizacion.API/Cotizacion.API.csproj \
     ./CotizacionMicroservice/src/Cotizacion.API/

RUN dotnet restore ./CotizacionMicroservice/CotizacionMicroservice.slnx

# Copy full source and publish
COPY CotizacionMicroservice/src ./CotizacionMicroservice/src

RUN dotnet publish ./CotizacionMicroservice/src/Cotizacion.API/Cotizacion.API.csproj \
      --configuration Release \
      --output /app/publish

# ─────────────────────────────────────────────────────────────────
# Stage 2 — Runtime (smallest possible image)
# ─────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# libgssapi-krb5-2 is required by Npgsql for GSSAPI negotiation with PostgreSQL
RUN apt-get update \
 && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
 && rm -rf /var/lib/apt/lists/*

# Non-root user for security
RUN groupadd --system --gid 1001 appgroup \
 && useradd  --system --uid 1001 --gid appgroup --no-create-home appuser \
 && mkdir -p /app/logs \
 && chown -R appuser:appgroup /app

COPY --from=build --chown=appuser:appgroup /app/publish .

USER appuser

# Listen on HTTP only — TLS is terminated at the reverse proxy level
ENV ASPNETCORE_URLS=http://+:3003
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 3003

ENTRYPOINT ["dotnet", "Cotizacion.API.dll"]
