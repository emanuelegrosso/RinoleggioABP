FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
ARG OPENIDDICT_CERTIFICATE_PASSPHRASE=5df40b5d-7941-4720-b8b7-12580278f056

# Fly build context must be the repository root (RinoleggioABP).
COPY . .

RUN dotnet dev-certs https -v -ep src/Rinoleggio.Web/openiddict.pfx -p "$OPENIDDICT_CERTIFICATE_PASSPHRASE"

RUN dotnet publish src/Rinoleggio.Web/Rinoleggio.Web.csproj \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /src/Rinoleggio.db /Rinoleggio.db

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Rinoleggio.Web.dll"]
