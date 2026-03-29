# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["AI.API.Manager.sln", "."]
COPY ["AI.API.Manager.Domain/AI.API.Manager.Domain.csproj", "AI.API.Manager.Domain/"]
COPY ["AI.API.Manager.Application/AI.API.Manager.Application.csproj", "AI.API.Manager.Application/"]
COPY ["AI.API.Manager.Infrastructure/AI.API.Manager.Infrastructure.csproj", "AI.API.Manager.Infrastructure/"]
COPY ["AI.API.Manager.API/AI.API.Manager.API.csproj", "AI.API.Manager.API/"]
COPY ["AI.API.Manager.Tests/AI.API.Manager.Tests.csproj", "AI.API.Manager.Tests/"]

# Restore dependencies
RUN dotnet restore "AI.API.Manager.sln"

# Copy everything else and build
COPY . .
WORKDIR "/src/AI.API.Manager.API"
RUN dotnet build "AI.API.Manager.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AI.API.Manager.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install SQL Server tools for health checks (optional)
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AI.API.Manager.API.dll"]