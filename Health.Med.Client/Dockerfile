# Dockerfile: Health.Med.Client/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiando os projetos necessários
COPY Health.Med.Client/Client.Api/Client.Api.csproj Health.Med.Client/Client.Api/
COPY Health.Med.Exception/Health.Med.Exceptions/Health.Med.Exceptions.csproj Health.Med.Exception/Health.Med.Exceptions/

# Restaura os pacotes
RUN dotnet restore Health.Med.Client/Client.Api/Client.Api.csproj

# Copia o restante dos arquivos
COPY . .

# Build do projeto
WORKDIR /src/Health.Med.Client
RUN dotnet build Client.Api/Client.Api.csproj -c Release -o /app/build

FROM build AS publish
WORKDIR /src/Health.Med.Client
RUN dotnet publish Client.Api/Client.Api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Client.Api.dll"]
