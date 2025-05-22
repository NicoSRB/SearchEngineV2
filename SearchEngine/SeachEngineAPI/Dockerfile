# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SeachEngineAPI/SeachEngineAPI.csproj", "SeachEngineAPI/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "SeachEngineAPI/SeachEngineAPI.csproj"
COPY . .
WORKDIR "/src/SeachEngineAPI"
RUN dotnet build "SeachEngineAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SeachEngineAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SeachEngineAPI.dll"]
