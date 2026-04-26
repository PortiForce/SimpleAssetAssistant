# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
ARG APP_UID=1654
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Backend/Workers/Portiforce.SAA.Notifications.Worker.Host/Portiforce.SAA.Notifications.Worker.Host.csproj", "Backend/Workers/Portiforce.SAA.Notifications.Worker.Host/"]
COPY ["Backend/Workers/Portiforce.SAA.Notifications.Worker/Portiforce.SAA.Notifications.Worker.csproj", "Backend/Workers/Portiforce.SAA.Notifications.Worker/"]
COPY ["Backend/App/Portiforce.SAA.Application/Portiforce.SAA.Application.csproj", "Backend/App/Portiforce.SAA.Application/"]
COPY ["Backend/Infra/Portiforce.SAA.Infrastructure.EF/Portiforce.SAA.Infrastructure.EF.csproj", "Backend/Infra/Portiforce.SAA.Infrastructure.EF/"]
COPY ["Backend/Infra/Portiforce.SAA.Infrastructure/Portiforce.SAA.Infrastructure.csproj", "Backend/Infra/Portiforce.SAA.Infrastructure/"]
COPY ["Backend/Core/Portiforce.SAA.Core.Activities/Portiforce.SAA.Core.Activities.csproj", "Backend/Core/Portiforce.SAA.Core.Activities/"]
COPY ["Backend/Core/Portiforce.SAA.Core.Assets/Portiforce.SAA.Core.Assets.csproj", "Backend/Core/Portiforce.SAA.Core.Assets/"]
COPY ["Backend/Core/Portiforce.SAA.Core.Identity/Portiforce.SAA.Core.Identity.csproj", "Backend/Core/Portiforce.SAA.Core.Identity/"]
COPY ["Backend/Core/Portiforce.SAA.Core/Portiforce.SAA.Core.csproj", "Backend/Core/Portiforce.SAA.Core/"]
RUN dotnet restore "./Backend/Workers/Portiforce.SAA.Notifications.Worker.Host/Portiforce.SAA.Notifications.Worker.Host.csproj"
COPY . .
WORKDIR "/src/Backend/Workers/Portiforce.SAA.Notifications.Worker.Host"
RUN dotnet build "./Portiforce.SAA.Notifications.Worker.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Portiforce.SAA.Notifications.Worker.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Portiforce.SAA.Notifications.Worker.Host.dll"]
