# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["LogBook.Web.csproj", "./"]
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Create non-root user for security
RUN groupadd -r logbook && useradd -r -g logbook logbook

# Create data directory
RUN mkdir -p /app/data && chown -R logbook:logbook /app/data

# Copy published app
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV LOGBOOK_DATA_PATH=/app/data
ENV LOGBOOK_USERNAME=admin
ENV LOGBOOK_PASSWORD=admin

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/login || exit 1

# Switch to non-root user
USER logbook

ENTRYPOINT ["dotnet", "LogBook.Web.dll"]
