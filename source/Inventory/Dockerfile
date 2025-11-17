# ========== BUILD STAGE ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Clear NuGet cache
ENV NUGET_PACKAGES=/root/.nuget/packages
RUN dotnet nuget locals all --clear

# Copy solution file
COPY eCommerceApi.sln .

# Copy SharedLibrary (chứa .proto files)
COPY source/SharedLibrary/SharedLibrarySolution/*.csproj ./source/SharedLibrary/SharedLibrarySolution/

# Copy Inventory gRPC project
COPY source/Inventory/InventoryService.gRPC/*.csproj ./source/Inventory/InventoryService.gRPC/

# Restore dependencies
RUN dotnet restore source/Inventory/InventoryService.gRPC/InventoryService.gRPC.csproj --ignore-failed-sources

# Copy full source code (including .proto files)
COPY source/SharedLibrary/ ./source/SharedLibrary/
COPY source/Inventory/ ./source/Inventory/

# Xóa NuGet.config nếu có
RUN find /src -name "NuGet.config" -type f -delete || true

# Build and publish
WORKDIR /src/source/Inventory/InventoryService.gRPC
RUN dotnet publish -c Release -o /app/publish --no-restore

# ========== RUNTIME STAGE ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl cho healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 8083
ENV ASPNETCORE_URLS=http://+:8083

ENTRYPOINT ["dotnet", "InventoryService.gRPC.dll"]