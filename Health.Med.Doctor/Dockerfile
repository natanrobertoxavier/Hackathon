# Dockerfile: Health.Med.Doctor/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiando os projetos necessários
COPY Health.Med.Doctor/Doctor.Api/Doctor.Api.csproj Health.Med.Doctor/Doctor.Api/
COPY Health.Med.Exception/Health.Med.Exceptions/Health.Med.Exceptions.csproj Health.Med.Exception/Health.Med.Exceptions/

# Restaura os pacotes
RUN dotnet restore Health.Med.Doctor/Doctor.Api/Doctor.Api.csproj

# Copia o restante dos arquivos
COPY . .

# Build do projeto
WORKDIR /src/Health.Med.Doctor
RUN dotnet build Doctor.Api/Doctor.Api.csproj -c Release -o /app/build

FROM build AS publish
WORKDIR /src/Health.Med.Doctor
RUN dotnet publish Doctor.Api/Doctor.Api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Doctor.Api.dll"]