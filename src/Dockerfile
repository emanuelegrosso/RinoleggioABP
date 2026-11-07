# Build context is the repository's src/ directory (this is what Fly's GitHub integration uses).
# Local deploy: cd src && fly deploy
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Must match AuthServer:CertificatePassPhrase in Rinoleggio.Web/appsettings.json,
# otherwise the app crashes at startup ("certificate data cannot be read with the provided password").
ARG OPENIDDICT_CERTIFICATE_PASSPHRASE=8a337503-69aa-4e90-8e92-c5791cfc352b

COPY . .
# Project files import ../../common.props, which resolves to /common.props from /src/<Project>.
COPY common.props /common.props

# Generate the OpenIddict encryption/signing certificate.
# `dotnet dev-certs` can fail in some builders (exit code 3 = export error), so fall back to openssl.
RUN mkdir -p Rinoleggio.Web \
    && ( dotnet dev-certs https -ep Rinoleggio.Web/openiddict.pfx -p "$OPENIDDICT_CERTIFICATE_PASSPHRASE" \
      || ( echo "dev-certs failed, falling back to openssl" \
        && openssl req -x509 -newkey rsa:2048 -nodes -sha256 -days 3650 \
             -subj "/CN=localhost" \
             -addext "keyUsage=digitalSignature,keyEncipherment" \
             -keyout /tmp/openiddict.key -out /tmp/openiddict.crt \
        && openssl pkcs12 -export -out Rinoleggio.Web/openiddict.pfx \
             -inkey /tmp/openiddict.key -in /tmp/openiddict.crt \
             -passout "pass:$OPENIDDICT_CERTIFICATE_PASSPHRASE" ) ) \
    && test -s Rinoleggio.Web/openiddict.pfx

RUN test -f Rinoleggio.Web/Rinoleggio.Web.csproj \
    && dotnet publish Rinoleggio.Web/Rinoleggio.Web.csproj \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Seed copy of the SQLite database (src/seed/Rinoleggio.db). At runtime the live database is
# /data/Rinoleggio.db (persistent Fly volume mounted on /data); the entrypoint copies the seed
# there on first start. The default connection string ("../../Rinoleggio.db") would resolve to "/Rinoleggio.db".
RUN mkdir -p /app/seed /data
COPY --from=build /src/seed/Rinoleggio.db /app/seed/Rinoleggio.db
COPY --from=build /src/docker-entrypoint.sh /app/docker-entrypoint.sh
RUN sed -i 's/\r$//' /app/docker-entrypoint.sh && chmod +x /app/docker-entrypoint.sh

ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__Default="Data Source=/data/Rinoleggio.db;"
ENV App__SelfUrl=https://rinoleggioabp.fly.dev
ENV AuthServer__Authority=https://rinoleggioabp.fly.dev
EXPOSE 8080

ENTRYPOINT ["/app/docker-entrypoint.sh"]
