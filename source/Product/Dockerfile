# ========== BUILD STAGE ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Clear NuGet cache
ENV NUGET_PACKAGES=/root/.nuget/packages
RUN dotnet nuget locals all --clear

# Copy solution file
COPY eCommerceApi.sln .

# Copy SharedLibrary
COPY source/SharedLibrary/SharedLibrarySolution/*.csproj ./source/SharedLibrary/SharedLibrarySolution/

# Copy Inventory gRPC (dependency)
COPY source/Inventory/InventoryService.gRPC/*.csproj ./source/Inventory/InventoryService.gRPC/

# Copy Product project
COPY source/Product/ProductService.Presentation/*.csproj ./source/Product/ProductService.Presentation/

# Restore dependencies
RUN dotnet restore source/Product/ProductService.Presentation/ProductService.Presentation.csproj --ignore-failed-sources

# Copy full source code
COPY source/SharedLibrary/ ./source/SharedLibrary/
COPY source/Inventory/ ./source/Inventory/
COPY source/Product/ ./source/Product/

# XÓA appsettings từ InventoryService.gRPC để tránh conflict
RUN rm -f /src/source/Inventory/InventoryService.gRPC/appsettings*.json

# Xóa NuGet.config nếu có
RUN find /src -name "NuGet.config" -type f -delete || true

# Build and publish
WORKDIR /src/source/Product/ProductService.Presentation
RUN dotnet publish -c Release -o /app/publish --no-restore

# ========== RUNTIME STAGE ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8081

ENTRYPOINT ["dotnet", "ProductService.Presentation.dll"]