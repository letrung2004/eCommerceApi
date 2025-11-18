# ========== BUILD STAGE ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY eCommerceApi.sln .

# Copy SharedLibrary
COPY source/SharedLibrary/SharedLibrarySolution/*.csproj ./source/SharedLibrary/SharedLibrarySolution/

# Copy Inventory gRPC (dependency cho Order.Application)
COPY source/Inventory/InventoryService.gRPC/*.csproj ./source/Inventory/InventoryService.gRPC/

# Copy Order project files (all layers)
COPY source/Order/OrderService.Domain/*.csproj ./source/Order/OrderService.Domain/
COPY source/Order/OrderService.Application/*.csproj ./source/Order/OrderService.Application/
COPY source/Order/OrderService.Infrastructure/*.csproj ./source/Order/OrderService.Infrastructure/
COPY source/Order/OrderService.Presentation/*.csproj ./source/Order/OrderService.Presentation/

# Restore dependencies
WORKDIR /src/source/Order/OrderService.Presentation
RUN dotnet restore

# Copy full source code
WORKDIR /src
COPY source/SharedLibrary/ ./source/SharedLibrary/
COPY source/Inventory/ ./source/Inventory/
COPY source/Order/ ./source/Order/

# XÓA appsettings từ InventoryService.gRPC để tránh conflict
RUN rm -f /src/source/Inventory/InventoryService.gRPC/appsettings*.json

# Build and publish
WORKDIR /src/source/Order/OrderService.Presentation
RUN dotnet publish OrderService.Presentation.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========== RUNTIME STAGE ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8084
ENV ASPNETCORE_URLS=http://+:8084

ENTRYPOINT ["dotnet", "OrderService.Presentation.dll"]