# Dockerfile: Health.Med.User/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiando os projetos necessários
COPY Health.Med.User/User.Api/User.Api.csproj Health.Med.User/User.Api/
COPY Health.Med.Exception/Health.Med.Exceptions/Health.Med.Exceptions.csproj Health.Med.Exception/Health.Med.Exceptions/

# Restaura os pacotes
RUN dotnet restore Health.Med.User/User.Api/User.Api.csproj

# Copia o restante dos arquivos
COPY . .

# Build do projeto
WORKDIR /src/Health.Med.User
RUN dotnet build User.Api/User.Api.csproj -c Release -o /app/build

FROM build AS publish
WORKDIR /src/Health.Med.User
RUN dotnet publish User.Api/User.Api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "User.Api.dll"]
