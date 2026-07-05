# Multi-stage Dockerfile for EarlyLearner.Worker (builds in the SDK image, runs on ASP.NET runtime)

# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the solution file first
COPY backend/EarlyLearner.slnx ./backend/

# Copy all source code under backend (projects, tests, database)
COPY backend/ ./backend/

# Restore dependencies for the entire solution
RUN dotnet restore backend/EarlyLearner.slnx

# Publish the worker project
RUN dotnet publish backend/EarlyLearner.Worker/EarlyLearner.Worker.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Keep memory usage modest for local Docker development
ENV DOTNET_GCServer=0

# Copy published output from build stage
COPY --from=build /app/publish .

# Entry point
ENTRYPOINT ["dotnet", "EarlyLearner.Worker.dll"]
