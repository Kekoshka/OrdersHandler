FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем csproj-файлы
COPY ["PaymentService.WebApi/PaymentService.WebApi.csproj", "PaymentService.WebApi/"]
COPY ["PaymentService.DataAccess.Postgres/PaymentService.DataAccess.Postgres.csproj", "PaymentService.DataAccess.Postgres/"]
# Другие зависимости (если есть)

# Копируем локальные NuGet пакеты
COPY ["packages/", "/src/packages/"]

# Восстанавливаем пакеты
RUN dotnet restore "PaymentService.WebApi/PaymentService.WebApi.csproj" \
    --source /src/packages \
    --source https://api.nuget.org/v3/index.json

# Копируем все исходники
COPY PaymentService.WebApi/ PaymentService.WebApi/
COPY PaymentService.DataAccess.Postgres/ PaymentService.DataAccess.Postgres/
# ...

WORKDIR "/src/PaymentService.WebApi"
RUN dotnet build "PaymentService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PaymentService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.WebApi.dll"]