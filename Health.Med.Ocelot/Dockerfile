# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o csproj e restaura dependências
COPY Health.Med.Ocelot/Health.Med.Ocelot.csproj Health.Med.Ocelot/
RUN dotnet restore Health.Med.Ocelot/Health.Med.Ocelot.csproj

# Copia o restante dos arquivos do projeto
COPY Health.Med.Ocelot/ Health.Med.Ocelot/

# Publica o projeto
WORKDIR /src/Health.Med.Ocelot
RUN dotnet publish -c Release -o /app/publish

# Imagem final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Health.Med.Ocelot.dll"]
