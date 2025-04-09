FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiando os projetos
COPY ../Health.Med.Notification/Notification.Api/Notification.Api.csproj Health.Med.Notification/Notification.Api/
COPY ../Health.Med.Exception/Health.Med.Exceptions/Health.Med.Exceptions.csproj Health.Med.Exception/Health.Med.Exceptions/

# Restaura os pacotes
RUN dotnet restore Health.Med.Notification/Notification.Api/Notification.Api.csproj

# Copia todo o conteúdo
COPY .. .

# Build do projeto
WORKDIR /src/Health.Med.Notification
RUN dotnet build Notification.Api/Notification.Api.csproj -c Release -o /app/build

FROM build AS publish
WORKDIR /src/Health.Med.Notification
RUN dotnet publish Notification.Api/Notification.Api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notification.Api.dll"]