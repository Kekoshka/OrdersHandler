FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем csproj-файлы для восстановления зависимостей
COPY ["OrderService.WebApi/OrderService.WebApi.csproj", "OrderService.WebApi/"]
COPY ["OrderService.DataAccess.Postgres/OrderService.DataAccess.Postgres.csproj", "OrderService.DataAccess.Postgres/"]
# Если есть другие проекты, добавьте их аналогично

# Копируем локальные NuGet пакеты (находятся в корне контекста)
COPY ["packages/", "/src/packages/"]

# Восстанавливаем пакеты, указывая локальный источник и nuget.org
RUN dotnet restore "OrderService.WebApi/OrderService.WebApi.csproj" \
    --source /src/packages \
    --source https://api.nuget.org/v3/index.json

# Копируем все исходники проектов
COPY OrderService.WebApi/ OrderService.WebApi/
COPY OrderService.DataAccess.Postgres/ OrderService.DataAccess.Postgres/
# ... остальные проекты

WORKDIR "/src/OrderService.WebApi"
RUN dotnet build "OrderService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.WebApi.dll"]