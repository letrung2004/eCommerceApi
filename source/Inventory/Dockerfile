# ========== BUILD STAGE ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY eCommerceApi.sln .

# Copy SharedLibrary (chứa .proto files)
COPY source/SharedLibrary/SharedLibrarySolution/*.csproj ./source/SharedLibrary/SharedLibrarySolution/
COPY source/SharedLibrary/SharedLibrarySolution/Protos ./source/SharedLibrary/SharedLibrarySolution/Protos

# Copy Inventory gRPC project
COPY source/Inventory/InventoryService.gRPC/*.csproj ./source/Inventory/InventoryService.gRPC/

# Restore dependencies
WORKDIR /src/source/Inventory/InventoryService.gRPC
RUN dotnet restore

# Copy full source code (after restore)
WORKDIR /src
COPY source/SharedLibrary/SharedLibrarySolution ./source/SharedLibrary/SharedLibrarySolution
COPY source/Inventory/InventoryService.gRPC ./source/Inventory/InventoryService.gRPC

# Build and publish
WORKDIR /src/source/Inventory/InventoryService.gRPC
RUN dotnet publish InventoryService.gRPC.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========== RUNTIME STAGE ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install SQL Server tools for healthcheck and migration wait
RUN apt-get update && apt-get install -y curl apt-transport-https gnupg2 \
    && curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
    && curl https://packages.microsoft.com/config/debian/11/prod.list > /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y mssql-tools18 unixodbc-dev \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

ENV PATH="$PATH:/opt/mssql-tools18/bin"

COPY --from=build /app/publish .

# Copy migration files if needed
COPY --from=build /src/source/Inventory/InventoryService.gRPC/Migrations ./Migrations

# Copy entrypoint script
COPY source/Inventory/entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5001

ENTRYPOINT ["/app/entrypoint.sh"]