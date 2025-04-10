# Dockerfile: Health.Med.Ocelot/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiando os projetos necessários
COPY Health.Med.Ocelot/Health.Med.Ocelot.csproj Health.Med.Ocelot/

# Restaura os pacotes
RUN dotnet restore Health.Med.Ocelot/Health.Med.Ocelot.csproj

# Copia o restante dos arquivos
COPY . .

# Build do projeto
WORKDIR /src/Health.Med.Ocelot
RUN dotnet build Health.Med.Ocelot.csproj -c Release -o /app/build

FROM build AS publish
WORKDIR /src/Health.Med.Ocelot
RUN dotnet publish Health.Med.Ocelot.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ocelot.dll"]