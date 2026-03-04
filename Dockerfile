# ============================================
# Stage 1: Build
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore (allows Docker layer caching)
COPY SPSUL.csproj ./
RUN dotnet restore

# Copy everything and publish
COPY . ./
RUN dotnet publish SPSUL.csproj -c Release -o /app/publish --no-restore

# ============================================
# Stage 2: Runtime
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Install curl for health checks (must be done as root before switching to app user)
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Pre-create the DataProtection keys directory with correct ownership while still root.
# When the named Docker volume is first mounted and empty, Docker copies this directory
# (including ownership) into the volume — so 'app' user can write keys on first run.
RUN mkdir -p /home/app/.aspnet/DataProtection-Keys \
    && chown -R app:app /home/app/.aspnet

# Copy published app from build stage (runs as non-root app user)
COPY --from=build --chown=app:app /app/publish ./

# Copy and fix entrypoint
COPY --chown=app:app docker-entrypoint.sh ./
RUN chmod +x docker-entrypoint.sh && \
    # Remove Windows line endings if present
    sed -i 's/\r$//' docker-entrypoint.sh

# Switch to non-root user (aspnet image provides 'app' user)
USER app

EXPOSE 8080

# Health check against the actual running web app
HEALTHCHECK --interval=30s --timeout=5s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/ || exit 1

ENTRYPOINT ["./docker-entrypoint.sh"]
