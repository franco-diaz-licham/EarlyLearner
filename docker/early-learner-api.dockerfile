# Multi-stage Dockerfile for Alumno360.Api (builds in the SDK image, runs on ASP.NET runtime)

# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the solution file first
COPY backend/EarlyLearner.slnx ./backend/

# Copy all source code under backend (projects, tests, database)
COPY backend/ ./backend/

# Restore dependencies for the entire solution
RUN dotnet restore backend/EarlyLearner.slnx

# Publish the API project
RUN dotnet publish backend/EarlyLearner.Api/EarlyLearner.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Set URL for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_GCServer=0
EXPOSE 80

HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
    CMD curl -fsS http://localhost/health/ready || exit 1

# Copy published output from build stage
COPY --from=build /app/publish .

# Entry point
ENTRYPOINT ["dotnet", "EarlyLearner.Api.dll"]
