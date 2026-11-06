FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
ARG OPENIDDICT_CERTIFICATE_PASSPHRASE=5df40b5d-7941-4720-b8b7-12580278f056
ENV PATH="${PATH}:/root/.dotnet/tools"

RUN apt-get update \
    && apt-get install -y --no-install-recommends nodejs npm \
    && rm -rf /var/lib/apt/lists/*

COPY . .

RUN if [ -d /src/src/Rinoleggio.Web ]; then ln -s /src/src /solution-src; else ln -s /src /solution-src; fi

# When Fly uses the repository's /src folder as its build context, the shared
# common.props file is one directory above the context. Recreate its build settings.
RUN printf '%s\n' \
    '<Project>' \
    '  <PropertyGroup>' \
    '    <LangVersion>latest</LangVersion>' \
    '    <Version>1.0.0</Version>' \
    '    <NoWarn>$(NoWarn);CS1591</NoWarn>' \
    '    <NuGetAudit>false</NuGetAudit>' \
    '    <AbpProjectType>app</AbpProjectType>' \
    '  </PropertyGroup>' \
    '</Project>' > /common.props

RUN dotnet dev-certs https -v -ep /solution-src/Rinoleggio.Web/openiddict.pfx -p "$OPENIDDICT_CERTIFICATE_PASSPHRASE"
RUN dotnet tool install --global Volo.Abp.Studio.Cli --version 3.1.6 \
    && abp install-libs --working-directory /solution-src/Rinoleggio.Web

RUN dotnet publish /solution-src/Rinoleggio.Web/Rinoleggio.Web.csproj \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Rinoleggio.Web.dll"]
