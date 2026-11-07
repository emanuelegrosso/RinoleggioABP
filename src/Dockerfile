FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Must match AuthServer:CertificatePassPhrase in src/Rinoleggio.Web/appsettings.json,
# otherwise the app crashes at startup ("certificate data cannot be read with the provided password").
ARG OPENIDDICT_CERTIFICATE_PASSPHRASE=8a337503-69aa-4e90-8e92-c5791cfc352b

# Build context must be the repository root (ABP/Codex/Rinoleggio).
COPY . .

RUN dotnet dev-certs https -v -ep src/Rinoleggio.Web/openiddict.pfx -p "$OPENIDDICT_CERTIFICATE_PASSPHRASE"

RUN dotnet publish src/Rinoleggio.Web/Rinoleggio.Web.csproj \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Seed copy of the SQLite database. At runtime the live database is /data/Rinoleggio.db
# (persistent Fly volume mounted on /data); the entrypoint copies the seed there on first start.
# The default connection string ("../../Rinoleggio.db") would resolve to "/Rinoleggio.db".
RUN mkdir -p /app/seed /data
COPY --from=build /src/Rinoleggio.db /app/seed/Rinoleggio.db
COPY --from=build /src/src/docker-entrypoint.sh /app/docker-entrypoint.sh
RUN sed -i 's/\r$//' /app/docker-entrypoint.sh && chmod +x /app/docker-entrypoint.sh

ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__Default="Data Source=/data/Rinoleggio.db;"
ENV App__SelfUrl=https://rinoleggioabp.fly.dev
ENV AuthServer__Authority=https://rinoleggioabp.fly.dev
EXPOSE 8080

ENTRYPOINT ["/app/docker-entrypoint.sh"]
