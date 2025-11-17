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

# Copy Inventory gRPC (dependency của Product)
COPY source/Inventory/InventoryService.gRPC/*.csproj ./source/Inventory/InventoryService.gRPC/

# Copy Product (dependency của Basket)
COPY source/Product/ProductService.Presentation/*.csproj ./source/Product/ProductService.Presentation/

# Copy Basket project
COPY source/Basket/BasketService.Presentation/*.csproj ./source/Basket/BasketService.Presentation/

# Restore dependencies
RUN dotnet restore source/Basket/BasketService.Presentation/BasketService.Presentation.csproj --ignore-failed-sources

# Copy full source code
COPY source/SharedLibrary/ ./source/SharedLibrary/
COPY source/Inventory/ ./source/Inventory/
COPY source/Product/ ./source/Product/
COPY source/Basket/ ./source/Basket/

# Xóa NuGet.config nếu có
RUN find /src -name "NuGet.config" -type f -delete || true

# XÓA appsettings từ Product và Inventory (chỉ giữ của Basket)
RUN find /src/source/Product -name "appsettings*.json" -type f -delete || true
RUN find /src/source/Inventory -name "appsettings*.json" -type f -delete || true

# Build and publish
WORKDIR /src/source/Basket/BasketService.Presentation
RUN dotnet publish -c Release -o /app/publish --no-restore

# ========== RUNTIME STAGE ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8082
ENV ASPNETCORE_URLS=http://+:8082

ENTRYPOINT ["dotnet", "BasketService.Presentation.dll"]