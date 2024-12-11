#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["service/pv-main/main.api/Main.API.csproj", "service/pv-main/main.api/"]
COPY ["building/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "building/EventBus/EventBusRabbitMQ/"]
COPY ["building/EventBus/EventBus/EventBus.csproj", "building/EventBus/EventBus/"]
COPY ["building/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj", "building/EventBus/IntegrationEventLogEF/"]
COPY ["building/WebHostCustomisation/WebHost.Customisation.csproj", "building/WebHostCustomisation/"]
COPY ["service/pv-main/main.infrastructure.mysql/Main.Infrastructure.MySQL.csproj", "service/pv-main/main.infrastructure.mysql/"]
COPY ["service/pv-main/main.infrastructure/Main.Infrastructure.csproj", "service/pv-main/main.infrastructure/"]
COPY ["service/pv-main/main.core/Main.Core.csproj", "service/pv-main/main.core/"]
COPY ["service/pv-main/main.infrastructure.sql/Main.Infrastructure.SQL.csproj", "service/pv-main/main.infrastructure.sql/"]
COPY ["service/pv-main/main.service/Main.Services.csproj", "service/pv-main/main.service/"]
RUN dotnet restore "./service/pv-main/main.api/Main.API.csproj"
COPY . .
WORKDIR "/src/service/pv-main/main.api"
RUN dotnet build "./Main.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Main.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenRIMS.PV.Main.API.dll"]