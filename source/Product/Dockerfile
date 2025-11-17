# ========== BUILD STAGE ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY eCommerceApi.sln .

# Copy SharedLibrary
COPY source/SharedLibrary/SharedLibrarySolution/*.csproj ./source/SharedLibrary/SharedLibrarySolution/

# Copy Inventory gRPC project (dependency của Product)
COPY source/Inventory/InventoryService.gRPC/*.csproj ./source/Inventory/InventoryService.gRPC/

# Copy Product project
COPY source/Product/ProductService.Presentation/*.csproj ./source/Product/ProductService.Presentation/

# Restore dependencies
WORKDIR /src/source/Product/ProductService.Presentation
RUN dotnet restore

# Copy full source code (after restore)
WORKDIR /src
COPY source/SharedLibrary/SharedLibrarySolution ./source/SharedLibrary/SharedLibrarySolution
COPY source/Inventory/InventoryService.gRPC ./source/Inventory/InventoryService.gRPC
COPY source/Product/ProductService.Presentation ./source/Product/ProductService.Presentation

# Build and publish
WORKDIR /src/source/Product/ProductService.Presentation
RUN dotnet publish ProductService.Presentation.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========== RUNTIME STAGE ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8081

ENTRYPOINT ["dotnet", "ProductService.Presentation.dll"]


# docker build -f source/Product/Dockerfile -t product-service:latest .