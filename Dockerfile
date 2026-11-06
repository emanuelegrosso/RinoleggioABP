# Build context: repository root (ABP/Rinoleggio)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
ARG OPENIDDICT_CERTIFICATE_PASSPHRASE=5df40b5d-7941-4720-b8b7-12580278f056
ENV PATH="${PATH}:/root/.dotnet/tools"

RUN apt-get update \
    && apt-get install -y --no-install-recommends nodejs npm \
    && rm -rf /var/lib/apt/lists/*
RUN npm install --global yarn@1.22.22

COPY . .

RUN dotnet dev-certs https -v -ep src/Rinoleggio.Web/openiddict.pfx -p "$OPENIDDICT_CERTIFICATE_PASSPHRASE"
RUN dotnet tool install --global Volo.Abp.Studio.Cli --version 3.1.6 \
    && abp install-libs --working-directory src/Rinoleggio.Web

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
