# Dockerfile: Health.Med.Ocelot/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiando os projetos necessários
COPY Health.Med.Ocelot/Ocelot.Api/Ocelot.Api.csproj Health.Med.Ocelot/Ocelot.Api/

# Restaura os pacotes
RUN dotnet restore Health.Med.Ocelot/Ocelot.Api/Ocelot.Api.csproj

# Copia o restante dos arquivos
COPY . .

# Build do projeto
WORKDIR /src/Health.Med.Ocelot
RUN dotnet build Ocelot.Api/Ocelot.Api.csproj -c Release -o /app/build

FROM build AS publish
WORKDIR /src/Health.Med.Ocelot
RUN dotnet publish Ocelot.Api/Ocelot.Api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ocelot.Api.dll"]