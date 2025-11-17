# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
LABEL stage=build-env
WORKDIR /app

# Copy solution files first
COPY Directory.Packages.props /app/
COPY global.json /app/
COPY GrandNode.sln /app/

# Copy source code maintaining directory structure
COPY ./src/ /app/src/

# Restore dependencies
RUN dotnet restore GrandNode.sln

ARG GIT_COMMIT
ARG GIT_BRANCH

# Build the entire solution
RUN dotnet build GrandNode.sln -c Release \
    -p:SourceRevisionId=$GIT_COMMIT \
    -p:GitBranch=$GIT_BRANCH \
    --no-restore

# Publish Web project
RUN dotnet publish /app/src/Web/Grand.Web/Grand.Web.csproj -c Release -o ./build/release \
    -p:SourceRevisionId=$GIT_COMMIT \
    -p:GitBranch=$GIT_BRANCH \
    --no-restore --no-build

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

EXPOSE 8080
WORKDIR /app

# Copy published application first
COPY --from=build-env /app/build/release .

# Create App_Data directory structure and set permissions
RUN mkdir -p App_Data/DataProtectionKeys App_Data/Download App_Data/TempUploads && \
    chmod -R 777 App_Data

# Create a non-root user and set ownership
RUN adduser --disabled-password --gecos '' appuser && \
    chown -R appuser:appuser /app

# Switch to the non-root user
USER appuser

# Configure ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "Grand.Web.dll"]
