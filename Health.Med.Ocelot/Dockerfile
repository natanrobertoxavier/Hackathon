# Dockerfile: Health.Med.Ocelot/Dockerfile

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiando o csproj com o caminho correto (três níveis: pasta do repositório, pasta do projeto e o arquivo)
COPY Health.Med.Ocelot/Health.Med.Ocelot/Health.Med.Ocelot.csproj Health.Med.Ocelot/

# Restaura os pacotes
RUN dotnet restore Health.Med.Ocelot/Health.Med.Ocelot.csproj

# Copia o restante dos arquivos do projeto
COPY Health.Med.Ocelot/ Health.Med.Ocelot/

# Publica o projeto
WORKDIR /src/Health.Med.Ocelot
RUN dotnet publish Health.Med.Ocelot.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Health.Med.Ocelot.dll"]
